using Realms;
using System;
using System.Collections.Generic;

namespace Project_MatField.Models;

public class BookGroup : RealmObject
{
    public BookGroup() { Id = (Ulid.NewUlid()).ToString(); }
    public BookGroup(string name, 
        List<string> bookIds)
    {
        this.Id = (Ulid.NewUlid()).ToString();
        this.Name = name;
        this.BookIds = bookIds;
    }

    [PrimaryKey]
    public string Id { get; set; }
    public string Name { get; set; } = null!;
    public IList<string> BookIds { get; set; } = [];
}
