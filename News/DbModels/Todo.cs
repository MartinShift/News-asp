using System;
using System.ComponentModel.DataAnnotations;

namespace News.DbModels
{
    public class Todo
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public bool IsDone { get; set; }
        public DateTime? Date { get; set; }

        public string UserId { get; set; }
    }
}
