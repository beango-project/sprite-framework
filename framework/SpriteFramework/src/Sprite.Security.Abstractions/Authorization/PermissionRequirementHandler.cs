using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Sprite.Security.Permissions;

namespace Sprite.Security.Authorization
{
    public class PermissionRequirementHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly PermissionManager _permissionManager;

        public PermissionRequirementHandler(PermissionManager permissionManager)
        {
            _permissionManager = permissionManager;
        }

        //授权管理器进行授权
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            //TODO: 在这里进行授权确认和检查
            var isGranted = await _permissionManager.IsGrantedAsync(requirement.PermissionName, context.User);
            if (isGranted)
            {
                context.Succeed(requirement);
            }
            //NOTE: 继续进行链式调用，由其他授权处理程序执行自己的处理逻辑
        }
    }
}