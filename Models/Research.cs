using Realms;
using System;
using System.Collections.Generic;

namespace Project_MatField.Models;

public class Research : RealmObject
{
    public Research() { Id = (Ulid.NewUlid()).ToString(); }
    public Research(string subject,
        string resources,
        string comment,
        string text,
        List<string> attachedFileLinks,
        string parentGroupId)
    {
        this.Id = (Ulid.NewUlid()).ToString();
        this.Subject = subject;
        this.Resources = resources;
        this.Comment = comment;
        this.Text = text;
        this.AttachedFileLinks = attachedFileLinks;
        this.ParentGroupId = parentGroupId;
    }

    [PrimaryKey]
    public string Id { get; set; }
    public string Subject { get; set; } = null!;
    public string Resources { get; set; } = null!;
    public string Comment { get; set; } = null!;
    public string Text { get; set; } = null!;
    public IList<string> AttachedFileLinks { get; set; } = [];
    public string ParentGroupId { get; set; } = null!;
}
