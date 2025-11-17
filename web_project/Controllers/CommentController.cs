using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using web_project.Models;
using web_project.Services;

namespace web_project.Controllers
{
    public class CommentController : Controller
    {
        private CommentService commentService = new CommentService();

        //GET: /Comment
        public ActionResult Index()
        {
            List<Comment> comments = commentService.GetAllComments();
            HttpContext.Application["comments"] = comments;
            return View(comments);
        }

        //GET: /Comment/Details/:id
        public ActionResult Details(int id)
        {
            Comment comment = commentService.GetCommentById(id);
            return View(comment);
        }

        //POST: /Comment/AddComment
        [HttpPost]
        public ActionResult AddComment(Comment comment)
        {
            User loggedUser = (User)Session["user"];

            if(loggedUser == null || loggedUser.Role != UserRole.Tourist)
            {
                return RedirectToAction("Index", "Authentication");
            }

            List<Comment> comments = commentService.GetAllComments();
            int newId = comments.Any() ? comments.Max(u => u.Id) + 1 : 1;
            comment.Id = newId;
            comment.TouristId = loggedUser.Id;
            comment.Status = CommentStatus.Pending;

            commentService.AddComment(comment);
            HttpContext.Application["comments"] = commentService.GetAllComments();

            ArrangementService arrangementService = new ArrangementService();
            Arrangement arrangement = arrangementService.GetAllArrangements()
                     .FirstOrDefault(a => a.AccommodationId == comment.AccommodationId);

            return RedirectToAction("Details", "Home", new { id = arrangement.Id });
        }

        //POST: /Comment/Approve/:id
        [HttpPost]
        public ActionResult Approve(int id)
        {
            Comment comment = commentService.GetCommentById(id);

            if(comment != null)
            {
                comment.Status = CommentStatus.Approved;
                commentService.UpdateComment(comment);
                HttpContext.Application["comments"] = commentService.GetAllComments();
            }
            ArrangementService arrangementService = new ArrangementService();
            Arrangement arrangement = arrangementService.GetAllArrangements()
                     .FirstOrDefault(a => a.AccommodationId == comment.AccommodationId);

            return RedirectToAction("Details", "Home", new { id = arrangement.Id });
        }

        //POST: /Comemnt/Reject/:id
        public ActionResult Reject(int id)
        {
            Comment comment = commentService.GetCommentById(id);

            if(comment != null)
            {
                comment.Status = CommentStatus.Rejected;
                commentService.UpdateComment(comment);
                HttpContext.Application["comments"] = commentService.GetAllComments();
            }

            ArrangementService arrangementService = new ArrangementService();
            Arrangement arrangement = arrangementService.GetAllArrangements()
                     .FirstOrDefault(a => a.AccommodationId == comment.AccommodationId);


            return RedirectToAction("Details", "Home", new { id = arrangement.Id });
        }
    }
}