using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blogger.Enums
{
    public enum Setting
    {
        // Make sure that the members are match the Keys in Blogger.Strings > setting.names.json resource
        LoginSettingName,
        AuthTokenSettingName,
        AccessTokenSettingName,
        RefreshTokenSettingName,

        ClientIdSettingName,
        ClientSecretSettingName,
        RedirectUriSettingName,
        TokenRequestUrlSettingName,
        AuthorizationRequestUrlSettingName,
        PermissionsScopeSettingName,
    }
}
