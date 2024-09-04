using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniTrade.Tools;

namespace UniTrade.Controllers
{
[ApiController]
[Route("api/[controller]")]
public class PayController : ControllerBase
{
    private readonly PayHelper _payHelper;

    public PayController(PayHelper payHelper)
    {
        _payHelper = payHelper;
    }

    [HttpGet("get-wxpay-qr")]
    public async Task<IActionResult> GetWxPayQRCode()
    {
        try
        {
            var qrCodeUrl = await _payHelper.GenerateQRCodeAsync();
            return Ok(new { qrCodeUrl = qrCodeUrl });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Failed to generate QR code: " + ex.Message);
        }
    }

    [HttpGet("check-payment-status")]
    public async Task<IActionResult> CheckPaymentStatus()
    {
        try
        {
            var paymentStatus = await _payHelper.CheckPaymentStatusAsync();
            return Ok(new { status = paymentStatus });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Failed to check payment status: " + ex.Message);
        }
    }
}

}
