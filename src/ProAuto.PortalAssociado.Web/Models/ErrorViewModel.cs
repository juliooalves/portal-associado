namespace ProAuto.PortalAssociado.Web.Models;

public sealed class ErrorViewModel
{
    public ErrorViewModel(int statusCode, string titulo, string mensagem)
    {
        StatusCode = statusCode;
        Titulo = titulo;
        Mensagem = mensagem;
    }

    public int StatusCode { get; }

    public string Titulo { get; }

    public string Mensagem { get; }
}
