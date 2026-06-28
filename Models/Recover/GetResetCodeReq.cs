namespace EcoSENA.Api.Models.Recover
{
    public class GetResetCodeReq
    {
        public required string Documento { get; set; }
        public required string Email { get; set; }
    }
}
