using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Project_MatField.Models;
using Realms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MatField.ViewModels
{
    public class SearchServiceViewModel
    {
        private Realm? _dbContext;
        public SearchServiceViewModel()
        {
            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            InitializeDbContext();
            GetAllSearchEntities();
        }

        private void InitializeDbContext() =>
            _dbContext = (Application.Current as App)?._serviceProvider
                .GetService<Realm>()!;

        private void GetAllSearchEntities()
        {
            var allBooks = _dbContext?.All<Book>().ToList();
            var allResearches = _dbContext?.All<Research>().ToList();
            var allResearchGroups = _dbContext?.All<ResearchGroup>().ToList();
            var allBookGroups = _dbContext?.All<BookGroup>().ToList();

            allBooks?.ForEach(x => { SearchEntities.Add(new(x)); });
            allBookGroups?.ForEach(x => { SearchEntities.Add(new(x)); });
            allResearches?.ForEach(x => { SearchEntities.Add(new(x)); });
            allResearchGroups?.ForEach(x => { SearchEntities.Add(new(x)); });   
        }

        public ObservableCollection<SearchEntityViewModel> SearchEntities { get; set; } = null!;
    }
}
