using Blogger.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blogger.Core;
using Blogger.Strings;

namespace Blogger.ViewModels
{
    public class AuthorizationPageViewModel : BaseViewModel
    {
        public DelegateCommand AuthorizeCommand { get; private set; }
        public Action<object> AuthorizeCommandExecute { get; set; }

        public AuthorizationPageViewModel()
        {
            AuthorizeCommand = new DelegateCommand(
                p => {
                    if (AuthorizeCommandExecute != null)
                    {
                        AuthorizeCommandExecute(p);
                    }
                },
                () =>
                {
                    return true;
                });
        }
    }
}
