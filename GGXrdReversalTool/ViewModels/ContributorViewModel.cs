using System;
using System.Collections.Generic;
using GGXrdReversalTool.Contributors;

namespace GGXrdReversalTool.ViewModels;



public class ContributorGroupViewModel : ViewModelBase
{
    public String Name { get; set; }

    public IEnumerable<Contributor> Contributors { get; set; }
}