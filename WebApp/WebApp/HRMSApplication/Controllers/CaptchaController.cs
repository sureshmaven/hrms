using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using HRMSApplication.Helpers;

namespace HRMSApplication.Controllers
{
    public class CaptchaController : Controller
    {
        // GET: Captcha
        public ActionResult Index()
        {
            return View();
        }
        

        public ActionResult CreateCaptcha()
        {

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Expires = -1;
            // Create the CAPTCHA image
            string captchaText = GenerateRandomText();
            Bitmap bitmap = new Bitmap(150, 33);
            // Declare Random outside of the loops
            Random random = new Random();
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // g.Clear(Color.White);
                // Clear the background with pale blue color
                Color paleBlue = Color.FromArgb(225, 225, 225); // RGB values for pale blue
                g.Clear(paleBlue);
                Font font = new Font("Arial", 14, FontStyle.Bold);

                //g.DrawString(captchaText, font, Brushes.Black, new PointF(10, 10));
                Color[] colors = { Color.Red, Color.Green, Color.Blue, Color.Orange, Color.Purple };

                // Draw each character in a different color
                for (int i = 0; i < captchaText.Length; i++)
                {
                    // Choose a color based on the character index
                    Brush brush = new SolidBrush(colors[i % colors.Length]); // Cycles through colors
                    g.DrawString(captchaText[i].ToString(), font, brush, new PointF(10 + (i * 30), 10)); // Adjust spacing as needed
                }
                // Add noise (optional)
                //for (int i = 0; i < 5; i++)
                //{
                //    int x1 = random.Next(bitmap.Width);
                //    int y1 = random.Next(bitmap.Height);
                //    int x2 = random.Next(bitmap.Width);
                //    int y2 = random.Next(bitmap.Height);
                //    g.DrawLine(new Pen(Color.LightGray, 2), x1, y1, x2, y2);
                //}

                //// Optionally add some dots or shapes for more noise
                //for (int i = 0; i < 100; i++)
                //{
                //    int x = random.Next(bitmap.Width);
                //    int y = random.Next(bitmap.Height);
                //    g.FillEllipse(new SolidBrush(Color.LightGray), x, y, 2, 2);
                //}
            }

            // Save CAPTCHA text in session
            Session["Captcha"] = captchaText;
            LogInformation.Info("Captcha generated: " + captchaText + " | SessionID: " + Session.SessionID);


            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return File(ms.ToArray(), "image/png");
            }
        }

        private string GenerateRandomText(int length = 5)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] stringChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            return new string(stringChars);
        }
    }

}
