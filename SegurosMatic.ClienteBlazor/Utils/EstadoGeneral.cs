namespace SegurosMatic.ClienteBlazor.Utils
{
    public static class EstadoGeneral
    {
        private static string _baseApiUrl;

        public static void Initialize(IConfiguration configuration)
        {
            _baseApiUrl = configuration["ApiSettings:BaseApiUrl"];
        }

        public static string BaseApiUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_baseApiUrl))
                    throw new InvalidOperationException("EstadoGeneral no se ha inicializado");
                return _baseApiUrl;
            }
        }
    }
}