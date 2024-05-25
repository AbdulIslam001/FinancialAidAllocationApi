using FinancialAidAllocation.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FinancialAidAllocation.Controllers
{
    public class FacultyController : ApiController
    {
        FAAToolEntities db = new FAAToolEntities();


        [HttpGet]
        public HttpResponseMessage CanRateGrader()
        {
            try
            {
                var session = db.Sessions.OrderByDescending(sess => sess.id).FirstOrDefault();

                if (session !=null) 
                {
                    String Todaydate=DateTime.Now.Day.ToString() + "/" +
                        DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString();
                    List<String> date = Todaydate.Split('/').ToList();
                    List<String> EndDate = session.end_date.Split('/').ToList();

                    List<String> StartDate=session.start_date.Split('/').ToList();

                    if (int.Parse(StartDate[1])+3 <= int.Parse(date[1]) &&
                        int.Parse(StartDate[2]) <= int.Parse(date[2]) &&
                        int.Parse(date[1]) <= int.Parse(EndDate[1])
                        )
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "Can Rate Now");
                    }
                    else 
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Can't Rate Now");

                    }
                }
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage FacultyInfo(int id)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK,db.Faculties.Where(f=>f.facultyId==id).FirstOrDefault());
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage TeachersGraders(int id)
        {
            try
            {   
                var result = db.graders.Where(gr=>gr.facultyId==id).Join
                    (
                    db.Students,
                    gr=>gr.studentId,
                    s=>s.student_id,
                    (gr,s) =>new 
                    {
                        s
                    }
                    );
                if (result.ToList().Count<1) 
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                else {
                return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        [HttpPost]
        public HttpResponseMessage RateGraderPerformance(String facultyId, String studentId,String rate , string comment)
        {
            try
            {
                int fId = int.Parse(facultyId);
                int sId = int.Parse(studentId);

                var session = db.Sessions.OrderByDescending(sess => sess.id).FirstOrDefault();

                var result = db.graders.Where(f => f.facultyId==fId&& f.studentId==sId && f.session==session.session1 && f.feedback==null && f.feedback==null&& f.comment==null).FirstOrDefault();
                
                if (result!=null)
                {
                    result.feedback =rate.ToString();
                    result.comment = comment;
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }else 
                {
                    return Request.CreateResponse(HttpStatusCode.Found, "Already Rated");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse (HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
