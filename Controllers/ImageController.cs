using Microsoft.AspNetCore.Mvc;

using System.Drawing.Imaging;
using System.Drawing;

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
        image.Uri = randString();

        context.Imagens.Add(image);
        context.SaveChanges();

        
        return new {
            Status = "Sucess",
            Message = "Inserido com sucesso",
            Data = image.Uri
        };
    }

    private string randString()
    {
        int seed = unchecked((int)DateTime.Now.Ticks);
        Random rand = new Random(seed);

        byte[] randData = new byte[12];
        rand.NextBytes(randData);

        var Base64Url = Convert.ToBase64String(randData);
        return Base64Url.Replace('/', 'X');
    }

    [HttpGet("get/{uri}")]

    public object Get(string uri)
    {
        GaleriaContext context = new GaleriaContext();
        var img = context.Imagens.FirstOrDefault(x => x.Uri == uri);
        if (img == null)
        {
            return new {
                Status = "Fail",
                Message = "Imagem não encontrada."
            };
        }
        var bytes = img.Bytes;
        return File(bytes, "image/jpeg");
    }

    [HttpPost("effect")]

    public object Effect([FromBody]Base64Image img)
    {
        var imgBytes = Convert.FromBase64String(img.Image);

        MemoryStream ms = new MemoryStream(imgBytes);
        Bitmap bmp = Bitmap.FromStream(ms) as Bitmap;

        Graphics g = Graphics.FromImage(bmp);
        g.Clear(Color.Blue);

        ms = new MemoryStream();
        bmp.Save(ms, ImageFormat.Jpeg);

        GaleriaContext context = new GaleriaContext();

        Imagen image = new Imagen();
        image.Bytes = ms.GetBuffer();
        image.Title = img.Title;
        image.Uri = randString();

        context.Imagens.Add(image);
        context.SaveChanges();

        return new {
            Status = "Sucess",
            Message = "Dados salvos com sucesso no banco de dados.",
            Data = image.Uri
        };
    }

    [HttpPost("create/{wid}x{hei},({r},{g},{b}),{title}")]

    public object Create(int wid, int hei, int r, int g, int b, string title)
    {
        Bitmap bmp = new Bitmap(wid, hei);
        var graphic = Graphics.FromImage(bmp);

        var color = Color.FromArgb(r, g, b);
        graphic.Clear(color);

        MemoryStream ms = new MemoryStream();
        bmp.Save(ms, ImageFormat.Jpeg);

        Imagen img = new Imagen();
        img.Title = title;
        img.Uri = randString();
        img.Bytes = ms.GetBuffer();

        GaleriaContext context = new GaleriaContext();
        context.Imagens.Add(img);
        context.SaveChanges();

        return new {
            Status = "Sucess",
            Message = "Imagem criada com sucesso.",
            Data = img.Uri
        };
    }

    [HttpPost("drawline/{url},({x1},{y1}),({x2},{y2}),({r},{g},{b}),{wid}")]

    public object DrawLine(string url, int x1, int y1, int x2, int y2, int r, int g, int b, int wid)
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
        MemoryStream ms = new MemoryStream(img.Bytes);
        Bitmap bmp = Bitmap.FromStream(ms) as Bitmap;
        var graphic = Graphics.FromImage(bmp);

        var color = Color.FromArgb(r, g, b);
        var pen = new Pen(color, wid);
        graphic.DrawLine(pen, x1, y1, x2, y2);

        ms = new MemoryStream();
        bmp.Save(ms, ImageFormat.Jpeg);

        img.Bytes = ms.GetBuffer();
        context.SaveChanges();

        return new {
            Status = "Sucess",
            Message = "Imagem alterada com sucesso."
        };
    }

}