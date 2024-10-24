using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SPI_Printer_Middleware.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZebraPrinterController : ControllerBase
    {
        [HttpPost("imprimir")]
        public async Task<IActionResult> ImprimirEtiqueta([FromBody] ZebraPrintRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.IpImpressora) || request.Porta == 0 || string.IsNullOrEmpty(request.ComandosZPL))
            {
                return BadRequest("Parâmetros inválidos ou ausentes.");
            }

            try
            {
                // Conectar à impressora Zebra via TCP/IP
                using (var socket = new TcpClient(request.IpImpressora, request.Porta))
                {
                    using (var stream = socket.GetStream())
                    {
                        var comandosBytes = Encoding.UTF8.GetBytes(request.ComandosZPL);
                        await stream.WriteAsync(comandosBytes, 0, comandosBytes.Length);
                        await stream.FlushAsync();
                    }
                }

                return Ok("Comandos enviados com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao enviar comandos para a impressora: {ex.Message}");
            }
        }
    }

    public class ZebraPrintRequest
    {
        public required string IpImpressora { get; set; }
        public required int Porta { get; set; }
        public required string ComandosZPL { get; set; }
    }
}
