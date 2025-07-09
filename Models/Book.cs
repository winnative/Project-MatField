using Realms;
using System;

namespace Project_MatField.Models
{
    public class Book : Realms.RealmObject
    {
        public Book() { Id = (Ulid.NewUlid()).ToString(); }
        public Book(string code,
            string name,
            string writer,
            string translator,
            string publisher,
            int publishYear,
            int volume,
            int column,
            int row,
            int point,
            string parentGroupId,
            string subject)
        {
            this.Id = (Ulid.NewUlid()).ToString();
            this.Code = code;
            this.Name = name;
            this.Writer = writer;
            this.Translator = translator;
            this.Publisher = publisher;
            this.PublishYear = publishYear;
            this.Volume = volume;
            this.Column = column;
            this.Row = row;
            this.ParentGroupId = parentGroupId;
            this.Subject = subject;
        }

        [PrimaryKey]
        public string Id { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Writer { get; set; } = null!;
        public string Translator { get; set; } = null!;
        public string Publisher { get; set; } = null!;
        public int PublishYear { get; set; }
        public int Volume { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public string ParentGroupId { get; set; } = null!;
        public int Point { get; set; }
        public string Subject { get; set; } = null!;
    }
}
