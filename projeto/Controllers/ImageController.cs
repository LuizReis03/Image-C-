using Microsoft.AspNetCore.Mvc;

namespace projeto.Controllers;

using Model;

[ApiController]
[Route("image")]
public class ImageController : ControllerBase
{
    [HttpPost("save")]
    public object Save([FromBody]Base64Image img)
    {
        GaleriaContext context = new GaleriaContext();

        Imagen image = new Imagen();
        image.Bytes = Convert.FromBase64String(img.Image);
        image.Title = img.Title;
        image.Uri = "xyz";

        context.Imagens.Add(image);
        context.SaveChanges();

        
        return new {
            Status = "Sucess",
            Message = "Inserido com sucesso"
        };
    }

    [HttpGet("get/{url}")]

    public object Get(string url)
    {
        GaleriaContext context = new GaleriaContext();
        var img = context.Imagens.FirstOrDefault(x => x.Uri == url);
        if (img == null)
        {
            return new {
                Status = "Fail",
                Message = "Imagem não encontrada."
            };
        }
        return new {
                Status = "Sucess",
                Message = "Imagem retornada com sucesso.",
                Data = img
            };
    }

}