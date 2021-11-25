# 这是一个应用框架
**目前的文档是不全面的，文档也在更新中...**

你可以用它来开发Web app 或者 普通类型的app。
1. 框架整体都是以模块化来设计和开发的，支持模块化开发。
2. 设计了可替换的容器集成方式，内置使用.NET Core 原生的依赖注入框架实现自动注入。目前适配了DryIoc依赖注入框架以支持“接口方式”注入和“Attribute方式”注入。你也可以替换成为其他第三方依赖注入框架。
3. 支持AOP，可以选用动态代理或者静态织入方式进行。
4. Web应用集成和支持：目前可以使用ASP.NET CORE 和MVC，可以使用Identity成员系统，支持Web API动态生成。
5. ORM集成和支持:该功能被设计成为通用且可替换的，目前支持 EntityFrameworkCore，自动事务管理（由工作单元UnitOfWork提供和实现），
6. DDD支持：可以使用IRepository接口来进行CRUD,使用特定的模式Attribute来自动设定和赋值。例如：CreatedDate,CreatedBy, LastModifiedDate,LastModifiedBy...等
7. ID生成器：支持普通Guid生成和Snowflake算法生成，并且集成了Snowflake漂移算法提供更高性能的ID生成（提供经过优化设定的默认配置）。
8. 对象映射器：使用Mapster来提供ObjectMapper，Mapster的性能是目前所有对象映射器中最好的，在框架里已使用快速表达式编译，带来了更好的性能和更小的消耗。你也可以换成例如：AutoMapper或其他的。


**框架还在积极开发中，未来会提供更多的功能。**
## 开始

### 非ASP.NET CORE应用以及普通应用


    //先创建如下所示的模块配置类和模块类
    
    //模块配置类
    public class MyModuleConfig : ModuleConfig
    {
        public override void Configure()
        {   //导入需要使用的模块
            ImportModules(typeof(SpriteDataModule),typeof(SpriteEfCoreModule));
        }
    }
    
    //模块类
    [Usage(typeof(MyModuleConfig))] //使用模块配置，导入模块
    public class MyModule : SpriteModule
    {
        //就像在asp.net core 中一样使用
        public override void ConfigureServices(IServiceCollection services)
        {
            //services.AddXXX();
            //services.Configure<xxx>()
        }

        //该方法不是必须的，会自动从依赖注入容器中获取此方法里的参数并设置值。
        // public void Configre(T t...) //例如 DbContext dbContext,在Asp.net core中可能是:IApplicationBuilder app, IWebHostEnvironment env...
        //{
            //app.useXXX();
            //dbContext.Database.EnsureDeleted();
        //}
    }

    //自动注入方式一 
    public class Component1 : ITransientInjection //瞬态
    {
        public void Do()
        {
            
        }
        
    }
    
    //自动注入方式二
    [Component(ServiceLifetime.Singleton)] //单例
    public class Component2
    {
        
    }
    
    public class Program
    {
       public static void Main(string[] args)
        {
            var appContext=SpriteApplication.Build<MyModule>() //构建 Sprite App
            //var appContext = new ConventionalSpriteApplicationContext(typeof(MyModule)); //或使用此方法创建
            appContext.Run(); //运行
            //可以使用 appContext.ServiceProvider属性的方法来获取注入的服务和组件
            var component1 = appContext.ServiceProvider.GetService<Component1>();
            
            component1.Do();

            var component2 = appContext.ServiceProvider.GetRequiredService<Component2>();
            
            appContext.Shutdown(); //停止
        }
    }
