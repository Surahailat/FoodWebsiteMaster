using System.Net.Mail;
using System.Net;
using System.Text.Json;
using FoodWebsiteMaster.Models;
using FoodWebsiteMaster.Models.viewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodWebsiteMaster.Controllers
{
    public class MainController : Controller
    {
        private readonly MyDbContext _context;

        public MainController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult Home()
        {
            return View();
        }
        public async Task<IActionResult> Home2()
        {
            var recipes = await _context.Recipes
                                        .OrderByDescending(r => r.CreatedAt)
                                        .Take(6)
                                        .ToListAsync();

            var subscriptions = await _context.Subscriptions.ToListAsync();

            var model = new HomePageViewModel
            {
                Recipes = recipes,
                Subscriptions = subscriptions
            };

            return View(model);
        }


        //public async Task<IActionResult> Home2()
        //{
        //    return View(await _context.Products.ToListAsync());
        //}
        public IActionResult recipe()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult shop()
        {
            return View();
        }
        public IActionResult singleProduct()
        {
            return View();
        }
        public IActionResult mealPlan()
        {
            return View();
        }

        [HttpGet("Appointment/{doctorId}")]
        public async Task<IActionResult> Appointment(int doctorId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == doctorId);

            if (user == null || doctor == null)
            {
                return NotFound();
            }

            ViewBag.User = user;
            ViewBag.Doctor = doctor;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateAppointment(string name, string email, string message, IFormFile file, string doctorName, string doctorPosition)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            // تحقق من عدد الحجوزات لهذا الإيميل خلال هذا الشهر
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            var appointmentsCount = await _context.Appointments
                .Where(a => a.Email == email && a.AppointmentDate.Month == currentMonth && a.AppointmentDate.Year == currentYear)
                .CountAsync();

            if (appointmentsCount >= 2)
            {
                TempData["Error"] = "You can only book 2 appointments per month.";
                return RedirectToAction("Appointment");
            }

            var appointment = new Appointment
            {
                FullName = name,
                Email = email,
                Message = message,
                DoctorName = doctorName,
                DoctorPosition = doctorPosition,
                AppointmentDate = DateOnly.FromDateTime(DateTime.Today),
                FilePath = file != null ? await SaveFileAsync(file) : null,
                CreatedAt = DateTime.Now
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction("AppointmentConfirmation");
        }

        private async Task<string> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            // التحقق من نوع الملف (مثلاً PDF أو صورة)
            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
                throw new InvalidOperationException("File type not allowed.");

            // التحقق من حجم الملف (مثلاً لا يتجاوز 5 ميجابايت)
            const long maxFileSize = 5 * 1024 * 1024; // 5 MB
            if (file.Length > maxFileSize)
                throw new InvalidOperationException("File size exceeds the maximum allowed size.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/" + uniqueFileName;
        }



        public async Task<IActionResult> ourDoctors()
        {
            var doctors = await _context.Doctors.ToListAsync();
            return View(doctors);
        }

        public IActionResult cart()
        {
            return View();
        }

        public IActionResult contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendMessage(string Name, string Phone, string Email, string Subject, string Message)
        {
            var contactMessage = new Contact
            {
                Name = Name,
                Phone = Phone,
                Email = Email,
                Subject = Subject,
                Message = Message,
                //CreatedAt = DateTime.Now
            };

            _context.Contacts.Add(contactMessage);
            _context.SaveChanges();

            //////////////////////// I have a problem here it is not sending the email
            try
            {
                var mail = new MailMessage();
                mail.From = new MailAddress("hailatsura@gmail.com", "Livance");
                mail.To.Add("hailatsura@gmail.com");
                mail.Subject = $"{Subject}";
                mail.Body = $"الاسم: {Name} \nالبريد: {Email}\n\nالرسالة:\n{Message}";
                mail.ReplyToList.Add(new MailAddress(Email));

                var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("hailatsura@gmail.com", "iicgb ajvm hxdv deln"),
                    EnableSsl = true
                };

                smtp.Send(mail);
            }
            catch (SmtpException smtpEx)
            {
                ViewBag.Error = smtpEx.Message;
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View("contact");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Subscribe(Paymentssub payment)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("signIn", "user");
            }

            payment.UserId = userId.Value;
            payment.PaymentDate = DateTime.Now;
            payment.PaymentStatus = "Paid"; // أو حسب حالة الدفع الحقيقية

            _context.Paymentssubs.Add(payment);
            _context.SaveChanges();

            // إنشاء الاشتراك الفعلي
            var subscription = _context.Subscriptions.FirstOrDefault(s => s.Id == payment.SubscriptionId);
            if (subscription != null)
            {
                var userSubscribe = new UserSubscribe
                {
                    UserId = userId.Value,
                    SubscriptionId = payment.SubscriptionId,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(subscription.DurationInDays)
                };

                _context.UserSubscribes.Add(userSubscribe);
                _context.SaveChanges();
            }

            return RedirectToAction("Home2","Main");
        }

        public IActionResult Payment()
        {
            return View();
        }

        public IActionResult PaymentProcess(Payment payment)
        {
            return View();
        }
    }


    }
