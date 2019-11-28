using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using upload_file.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace upload_file.Controllers
{
    public class HomeController : Controller
    {
        private IHostingEnvironment hostingEnvironment;
        public HomeController(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IList<IFormFile> files)
        {
            Startup.Progress = 0;

            long totalBytes = files.Sum(f => f.Length);
            long totalReadBytes = 0;
            
            foreach (IFormFile source in files)
            {
                string filename = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.ToString().Trim('"');

                filename = this.EnsureCorrectFilename(filename);
                byte[] buffer = new byte[16 * 1024];

                string namafileExcell = "";
                string namafilenewExcell = "";
                string extension = "";

                namafileExcell = source.FileName;
                extension = namafileExcell.Substring(namafileExcell.LastIndexOf('.') + 1);
                namafileExcell = namafileExcell.Replace(namafileExcell.Substring(namafileExcell.LastIndexOf('.') + 1), "");
                namafilenewExcell = namafileExcell.Replace(".", "") + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "." + extension;

                var path = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot\\UploadFile",
                namafilenewExcell);

                if (System.IO.File.Exists(path))
                {
                    FileInfo f2 = new FileInfo(path);
                    f2.Delete();
                }

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await source.CopyToAsync(stream);
                }
                
                try
                {
                    int readBytes;
                    using (FileStream output = System.IO.File.Create(this.GetPathAndFilename(namafilenewExcell)))
                    {
                        using (Stream input = source.OpenReadStream())
                        {
                            while ((readBytes = input.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                await output.WriteAsync(buffer, 0, readBytes);
                                totalReadBytes += readBytes;
                                Startup.Progress = (int)((float)totalReadBytes / (float)totalBytes * 100.0);
                                await Task.Delay(10); // It is only to make the process slower
                            }
                        }
                    }
                }
                catch (Exception err)
                {
                    string msgError = "";
                    msgError = err.ToString();
                    Startup.Progress = (int)((float)totalReadBytes / (float)totalBytes * 100.0);
                    return Json(new { message = msgError });
                }


            }
            
            return Json(new { message = "Data Uploaded Successfully!" });
        }

        [HttpPost]
        public ActionResult Progress()
        {
            return this.Content(Startup.Progress.ToString());
        }

        private string EnsureCorrectFilename(string filename)
        {
            if (filename.Contains("\\"))
                filename = filename.Substring(filename.LastIndexOf("\\") + 1);

            return filename;
        }

        private string GetPathAndFilename(string filename)
        {
            string path = this.hostingEnvironment.WebRootPath + "\\FileUpload\\";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path + filename;
        }
    }
}
