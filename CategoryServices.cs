using SalesSystem.DAL;
using SalesSystem.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalesSystem.DAL;
using SalesSystem.viewModel;
using System.Collections;

namespace SalesSystem.BLL
{
    public class CategoryServices
    {
        #region Constructor and Context Dependency

        private readonly eToolsContext _context;
        internal CategoryServices(eToolsContext context)
        {
            _context = context;
        }
        #endregion
        public List<categoriesInfo> getCat()
        {
            IEnumerable <categoriesInfo> display = _context. Categories.Select(x => new  categoriesInfo{ categoryID = x. CategoryID, Description = x.Description });
            return display.ToList();
        }
    }
}
