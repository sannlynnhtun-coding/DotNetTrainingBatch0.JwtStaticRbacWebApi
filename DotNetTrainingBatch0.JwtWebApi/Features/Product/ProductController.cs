using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetTrainingBatch0.JwtWebApi.Features.Product;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [Authorize(Policy = "ProductView")]
    public IActionResult GetProducts([FromQuery] ProductListRequest request)
    {
        var response = _productService.GetProducts(request);
        return Ok(response);
    }

    [HttpPost]
    [Authorize(Policy = "ProductCreate")]
    public IActionResult CreateProduct(ProductCreateRequest request)
    {
        var response = _productService.CreateProduct(request);
        return Ok(response);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "ProductUpdate")]
    public IActionResult UpdateProduct(int id, ProductUpdateRequest request)
    {
        var response = _productService.UpdateProduct(id, request);
        if (!response.IsSuccess) return NotFound(response);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "ProductDelete")]
    public IActionResult DeleteProduct(int id)
    {
        var response = _productService.DeleteProduct(id);
        if (!response.IsSuccess) return NotFound(response);
        return Ok(response);
    }

    [HttpGet("admin-only")]
    [Authorize(Roles = "Admin")]
    public IActionResult AdminOnly()
    {
        return Ok(new AdminOnlyResponse { Message = "Only Admin can access this API." });
    }
}
