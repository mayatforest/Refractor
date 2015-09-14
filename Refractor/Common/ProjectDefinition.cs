using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Refractor.Common
{
    [Serializable]
    public class ProjectDefinition
    {
        private List<string> _filenames = new List<string>();
        public List<string> Filenames { get { return _filenames; } set { _filenames = value; } }

        private List<FavouritesItem> _favourites = new List<FavouritesItem>();
        public List<FavouritesItem> Favourites { get { return _favourites; } set { _favourites = value; } }

        private List<string> _favouritesPaths = new List<string>();
        public List<string> FavouritesPaths { get { return _favouritesPaths; } set { _favouritesPaths = value; } }

        private List<string> _windows = new List<string>();
        public List<string> Windows { get { return _windows; } set { _windows = value; } }

        private List<string> _items = new List<string>();
        public List<string> Items { get { return _items; } set { _items = value; } }

        private string _projectItem = string.Empty;
        public string ProjectItem { get { return _projectItem; } set { _projectItem = value; } }
    }
}
