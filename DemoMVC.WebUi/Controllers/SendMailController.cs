using DemoMVC.Models;
using DemoMVC.Service;
using System;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace DemoMVC.WebUi.Controllers
{
    public class SendMailController : BaseController
    {
        private readonly UserProfileService _userProfileService;
        private readonly UserExamService _userExamSerive;
        private readonly ExamService _examService;

        public SendMailController()
        {
            _userProfileService = new UserProfileService();
            _userExamSerive = new UserExamService();
            _examService = new ExamService();
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult SendExamEmail(int userExamId, string generatedLink)
        {
            try
            {
                var userExam = _userExamSerive.GetByUserExamId(userExamId);
                if (userExam == null)
                {
                    return Json(new { success = false, message = "Exam details not found." });
                }

                bool emailSent = SendEmail(userExam, generatedLink);
                return Json(new { success = emailSent, message = emailSent ? "Email sent successfully!" : "Failed to send email." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        private bool SendEmail(UserExams userExam, string generatedLink)
        {
            try
            {
                var userProfile = _userProfileService.GetUserById(userExam.UserId);
                var examDetails = _examService.GetById(userExam.ExamId);
                string smtpServer = "smtp.gmail.com";
                int smtpPort = 587;
                string smtpUser = "pateljainish2884@gmail.com";
                string smtpPass = "qvbc gcij nkny ezoz";

                string subject = "Your Exam Details & Login Link - " + examDetails.ExamName;

                string body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ width: 90%; max-width: 600px; margin: auto; border: 1px solid #ddd; padding: 20px; border-radius: 10px; background: #f9f9f9; }}
                        .header {{ text-align: center; font-size: 18px; font-weight: bold; color: #0056b3; }}
                        .details-table {{ width: 100%; border-collapse: collapse; margin-top: 10px; }}
                        .details-table th, .details-table td {{ border: 1px solid #ddd; padding: 10px; text-align: left; }}
                        .details-table th {{ background-color: #f2f2f2; }}
                        .instructions {{ margin-top: 15px; padding: 10px; background: #fffbcc; border-left: 5px solid #f1c40f; }}
                        .exam-link {{ display: block; text-align: center; margin: 20px 0; }}
                        .cta-button {{ background-color: #28a745; color: white; padding: 10px 15px; text-decoration: none; border-radius: 5px; font-weight: bold; }}
                        .footer {{ text-align: center; font-size: 12px; margin-top: 20px; color: #666; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <p class='header'>Dear Candidate,</p>
                        <p>We are pleased to inform you that you have been registered for the following exam. Below are the details:</p>
        
                        <table class='details-table'>
                            <tr>
                                <th>Exam Name</th>
                                <td>{examDetails.ExamName}</td>
                            </tr>
                            <tr>
                                <th>Total Marks</th>
                                <td>{examDetails.TotalMarks}</td>
                            </tr>
                            <tr>
                                <th>Passing Marks</th>
                                <td>{examDetails.PassingMarks}</td>
                            </tr>
                            <tr>
                                <th>Duration</th>
                                <td>{examDetails.DurationMin} minutes</td>
                            </tr>
                        </table>

                        <div class='instructions'>
                            <strong>Important Instructions:</strong>
                            <ul>
                                <li>Ensure you have a stable internet connection before starting the exam.</li>
                                <li>Once started, you cannot pause or restart the exam.</li>
                                <li>Do not refresh or close the browser during the exam.</li>
                                <li>Make sure to submit the exam before the timer runs out.</li>
                            </ul>
                        </div>

                        <div class='exam-link'>
                            <p><strong>Click the button below to start your exam:</strong></p>
                            <a class='cta-button' href='{generatedLink}' target='_blank'>Start Exam</a>
                        </div>

                        <p>If the button above does not work, copy and paste this link in your browser:</p>
                        <p><a href='{generatedLink}' target='_blank'>{generatedLink}</a></p>

                        <div class='footer'>
                            <p>Best Regards,<br>Exam Support Team</p>
                            <p><small>If you did not request this exam, please ignore this email.</small></p>
                        </div>
                    </div>
                </body>
                </html>";

                // Email Configuration
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(smtpUser);
                mail.To.Add(userProfile.Email);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient(smtpServer, smtpPort);
                smtp.Credentials = new NetworkCredential(smtpUser, smtpPass);
                smtp.EnableSsl = true;
                smtp.Send(mail);

                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Email Error: " + ex.Message);
                return false;
            }
        }
    }
}