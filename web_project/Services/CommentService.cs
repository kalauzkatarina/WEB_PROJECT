using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using web_project.Models;

namespace web_project.Services
{
    public class CommentService
    {
        private readonly string _filePath = "~/App_data/comments.json";

        public List<Comment> GetAllComments()
        {
            var path = HostingEnvironment.MapPath(_filePath);

            if (!File.Exists(path))
            {
                return new List<Comment>();
            }

            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<Comment>>(json);
        }

        public Comment GetCommentById(int id)
        {
            List<Comment> comments = GetAllComments();
            foreach (var comment in comments)
            {
                if (comment.Id == id)
                {
                    return comment;
                }
            }
            return new Comment();
        }

        public void AddComment(Comment comment)
        {
            List<Comment> comments = GetAllComments();
            comments.Add(comment);
            SaveComments(comments);
        }

        public void UpdateComment(Comment updateComment)
        {
            List<Comment> comments = GetAllComments();
            int i;
            for (i = 0; i < comments.Count; i++)
            {
                if (comments[i].Id == updateComment.Id)
                {
                    comments[i] = updateComment;
                    break;
                }
            }
            SaveComments(comments);
        }

        private void SaveComments(List<Comment> comments)
        {
            var path = HostingEnvironment.MapPath(_filePath);
            var json = JsonConvert.SerializeObject(comments, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}