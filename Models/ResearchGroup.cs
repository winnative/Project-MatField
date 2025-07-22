using Realms;
using System;
using System.Collections.Generic;

namespace Project_MatField.Models;

public class ResearchGroup : RealmObject
{
    public ResearchGroup() { Id = (Ulid.NewUlid()).ToString(); }
    public ResearchGroup(string name,
        List<string> ResearchIds)
    {
        this.Id = (Ulid.NewUlid()).ToString();
        this.Name = name;
        this.ResearchIds = ResearchIds;
    }

    [PrimaryKey]
    public string Id { get; set; }
    public string Name { get; set; } = null!;
    public IList<string> ResearchIds { get; set; } = [];
}
