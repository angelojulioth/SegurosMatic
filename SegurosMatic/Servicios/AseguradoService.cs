using CsvHelper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using SegurosMaticAPI.Modelos;
using SegurosMaticAPI.Servicios.Interfaces;
using System.Data;
using System.Formats.Asn1;
using System.Globalization;

namespace SegurosMaticAPI.Servicios;

public class AseguradoService : IAseguradoService
{
    private readonly string _connectionString;
    private readonly ILogger<AseguradoService> _logger;

    public AseguradoService(IConfiguration configuration, ILogger<AseguradoService> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _logger = logger;
    }

    public async Task<IEnumerable<Asegurado>> ObtenerTodosAsync()
    {
        var asegurados = new List<Asegurado>();

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand("SELECT * FROM Asegurados", connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var asegurado = new Asegurado
                        {
                            Id = reader.GetInt32(0),
                            Cedula = reader.GetString(1),
                            Nombre = reader.GetString(2),
                            Telefono = reader.GetString(3),
                            Edad = reader.GetInt32(4),
                            Seguros = new List<Seguro>()
                        };

                        asegurados.Add(asegurado);
                    }
                }
            }

            foreach (var asegurado in asegurados)
            {
                using (var command = new SqlCommand("sp_ObtenerSegurosDeAseguradoId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@AseguradoId", asegurado.Id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var seguro = new Seguro
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("SeguroId")),
                                Nombre = reader.GetString(reader.GetOrdinal("NombreSeguro")),
                                Codigo = reader.GetString(reader.GetOrdinal("Codigo")),
                                SumaAsegurada = reader.GetDecimal(reader.GetOrdinal("SumaAsegurada")),
                                Prima = reader.GetDecimal(reader.GetOrdinal("Prima"))
                            };

                            asegurado.Seguros.Add(seguro);
                        }
                    }
                }
            }
        }

        return asegurados;
    }



    public async Task<Asegurado> ObtenerPorIdAsync(int id)
    {
        Asegurado asegurado = null;

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand("sp_ObtenerAsegurado", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        asegurado = new Asegurado
                        {
                            Id = reader.GetInt32(0),
                            Cedula = reader.GetString(1),
                            Nombre = reader.GetString(2),
                            Telefono = reader.GetString(3),
                            Edad = reader.GetInt32(4)
                        };
                    }
                }


            }
            // TODO refactorizar a otro método

            using (var command = new SqlCommand("sp_ObtenerSegurosDeAseguradoId", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@AseguradoId", asegurado.Id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var seguro = new Seguro
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("SeguroId")),
                            Nombre = reader.GetString(reader.GetOrdinal("NombreSeguro")),
                            Codigo = reader.GetString(reader.GetOrdinal("Codigo")),
                            SumaAsegurada = reader.GetDecimal(reader.GetOrdinal("SumaAsegurada")),
                            Prima = reader.GetDecimal(reader.GetOrdinal("Prima"))
                        };

                        asegurado.Seguros.Add(seguro);
                    }
                }
            }
        }

        return asegurado;
    }


    public async Task<Asegurado> ObtenerPorCedulaAsync(string cedula)
    {
        Asegurado asegurado = null;

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand("sp_BuscarAseguradoPorCedula", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Cedula", cedula);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        asegurado = new Asegurado
                        {
                            Id = reader.GetInt32(0),
                            Cedula = reader.GetString(1),
                            Nombre = reader.GetString(2),
                            Telefono = reader.GetString(3),
                            Edad = reader.GetInt32(4)
                        };
                    }
                }


            }

            if (asegurado == null)
                return new Asegurado(); // refactorizar dentro de un envoltorio para devolver un mensaje acorde
            // TODO refactorizar a otro método

            using (var command = new SqlCommand("sp_ObtenerSegurosDeAseguradoId", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@AseguradoId", asegurado.Id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var seguro = new Seguro
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("SeguroId")),
                            Nombre = reader.GetString(reader.GetOrdinal("NombreSeguro")),
                            Codigo = reader.GetString(reader.GetOrdinal("Codigo")),
                            SumaAsegurada = reader.GetDecimal(reader.GetOrdinal("SumaAsegurada")),
                            Prima = reader.GetDecimal(reader.GetOrdinal("Prima"))
                        };

                        asegurado.Seguros.Add(seguro);
                    }
                }
            }
        }


        return asegurado;
    }

    public async Task<int> CrearAsync(Asegurado asegurado)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand("sp_CrearAsegurado", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Cedula", asegurado.Cedula);
                command.Parameters.AddWithValue("@Nombre", asegurado.Nombre);
                command.Parameters.AddWithValue("@Telefono", asegurado.Telefono);
                command.Parameters.AddWithValue("@Edad", asegurado.Edad);
                var idParam = new SqlParameter("@Id", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(idParam);

                await command.ExecuteNonQueryAsync();

                return (int)idParam.Value;
            }
        }
    }

    public async Task BorrarSegurosDeAsegurado(int aseguradoId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand("sp_BorrarSegurosDeAsegurado", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@AseguradoId", aseguradoId);

                await command.ExecuteNonQueryAsync();
            }
        }
    }

    public async Task ActualizarAsync(Asegurado asegurado)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand("sp_ActualizarAsegurado", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Id", asegurado.Id);
                command.Parameters.AddWithValue("@Cedula", asegurado.Cedula);
                command.Parameters.AddWithValue("@Nombre", asegurado.Nombre);
                command.Parameters.AddWithValue("@Telefono", asegurado.Telefono);
                command.Parameters.AddWithValue("@Edad", asegurado.Edad);

                await command.ExecuteNonQueryAsync();
            }
        }
    }


    public async Task EliminarAsync(int id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand("sp_EliminarAsegurado", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", id);

                await command.ExecuteNonQueryAsync();
            }
        }
    }


    public async Task AsignarSeguroAsync(int aseguradoId, int seguroId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand("sp_AsignarSeguroAAsegurado", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@AseguradoId", aseguradoId);
                command.Parameters.AddWithValue("@SeguroId", seguroId);

                await command.ExecuteNonQueryAsync();
            }
        }
    }


    public async Task CargarAseguradosMasivoAsync(Stream archivo)
    {
        using (var reader = new StreamReader(archivo))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var registros = csv.GetRecords<AseguradoCarga>().ToList();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                foreach (var registro in registros)
                {
                    var asegurado = new Asegurado
                    {
                        Cedula = registro.Cedula,
                        Nombre = registro.Nombre,
                        Telefono = registro.Telefono,
                        Edad = registro.Edad
                    };

                    var id = await CrearAsync(asegurado);

                    // Asignar seguros según la edad
                    var seguros = await DeterminarSegurosPorEdad(connection, asegurado.Edad);
                    foreach (var seguro in seguros)
                    {
                        await AsignarSeguroAsync(id, seguro.Id);
                    }
                }
            }
        }
    }



    private async Task<List<Seguro>> DeterminarSegurosPorEdad(SqlConnection connection, int edad)
    {
        var seguros = new List<Seguro>();

        using (var command = new SqlCommand("SELECT Id, Nombre, Codigo, SumaAsegurada, Prima, EdadMinima, EdadMaxima FROM Seguros WHERE @Edad BETWEEN EdadMinima AND EdadMaxima", connection))
        {
            command.Parameters.AddWithValue("@Edad", edad);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var seguro = new Seguro
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                        Codigo = reader.GetString(reader.GetOrdinal("Codigo")),
                        SumaAsegurada = reader.GetDecimal(reader.GetOrdinal("SumaAsegurada")),
                        Prima = reader.GetDecimal(reader.GetOrdinal("Prima")),
                        EdadMinima = reader.GetInt32(reader.GetOrdinal("EdadMinima")),
                        EdadMaxima = reader.GetInt32(reader.GetOrdinal("EdadMaxima"))
                    };

                    seguros.Add(seguro);
                }
            }
        }

        return seguros;
    }


}
