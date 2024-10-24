using Microsoft.Data.SqlClient;
using SegurosMaticAPI.Modelos;
using SegurosMaticAPI.Servicios.Interfaces;
using System.Data;

namespace SegurosMaticAPI.Servicios
{
    public class SeguroService : ISeguroService
    {
        private readonly string _connectionString;
        private readonly ILogger<SeguroService> _logger;

        public SeguroService(IConfiguration configuration, ILogger<SeguroService> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        // metodo para obtener todos los registros de la tabla "Seguro"
        public async Task<IEnumerable<Seguro>> ObtenerTodosAsync()
        {
            var seguros = new List<Seguro>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_ListarSeguros", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            seguros.Add(new Seguro
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Codigo = reader.GetString(2),
                                SumaAsegurada = reader.GetDecimal(3),
                                Prima = reader.GetDecimal(4),
                                EdadMinima = reader.GetInt32(5),
                                EdadMaxima = reader.GetInt32(6)
                            });
                        }
                    }
                }
            }

            return seguros;
        }

        // metodo para obtener un unico seguro por su id
        public async Task<Seguro> ObtenerPorIdAsync(int id)
        {
            Seguro seguro = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_ObtenerSeguro", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            seguro = new Seguro
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Codigo = reader.GetString(2),
                                SumaAsegurada = reader.GetDecimal(3),
                                Prima = reader.GetDecimal(4),
                                EdadMinima = reader.GetInt32(5),
                                EdadMaxima = reader.GetInt32(6)
                            };
                        }
                    }
                }
            }

            return seguro;
        }

        // metodo para crear un nuevo registro de seguro
        public async Task<int> CrearAsync(Seguro seguro)
        {
            int newId;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_CrearSeguro", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Nombre", seguro.Nombre);
                    command.Parameters.AddWithValue("@Codigo", seguro.Codigo);
                    command.Parameters.AddWithValue("@SumaAsegurada", seguro.SumaAsegurada);
                    command.Parameters.AddWithValue("@Prima", seguro.Prima);
                    command.Parameters.AddWithValue("@EdadMinima", seguro.EdadMinima);
                    command.Parameters.AddWithValue("@EdadMaxima", seguro.EdadMaxima);

                    var outputParam = new SqlParameter("@Id", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputParam);

                    await command.ExecuteNonQueryAsync();

                    newId = (int)outputParam.Value;
                }
            }

            return newId;
        }

        // actualizar un registro actual de un seguro
        public async Task ActualizarAsync(Seguro seguro)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_ActualizarSeguro", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Id", seguro.Id);
                    command.Parameters.AddWithValue("@Nombre", seguro.Nombre);
                    command.Parameters.AddWithValue("@Codigo", seguro.Codigo);
                    command.Parameters.AddWithValue("@SumaAsegurada", seguro.SumaAsegurada);
                    command.Parameters.AddWithValue("@Prima", seguro.Prima);
                    command.Parameters.AddWithValue("@EdadMinima", seguro.EdadMinima);
                    command.Parameters.AddWithValue("@EdadMaxima", seguro.EdadMaxima);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        // méotod para borrar un seguro por su id
        public async Task EliminarAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_EliminarSeguro", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", id);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        // método para obtener todas las personas aseguradas mediante el código de seguro
        public async Task<IEnumerable<Asegurado>> ObtenerAseguradosPorSeguroAsync(string codigoSeguro)
        {
            var asegurados = new List<Asegurado>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_BuscarAseguradosPorSeguro", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CodigoSeguro", codigoSeguro);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            asegurados.Add(new Asegurado
                            {
                                Id = reader.GetInt32(0),
                                Cedula = reader.GetString(1),
                                Nombre = reader.GetString(2),
                                Telefono = reader.GetString(3),
                                Edad = reader.GetInt32(4)
                            });
                        }
                    }
                }
            }

            return asegurados;
        }
    }
}
