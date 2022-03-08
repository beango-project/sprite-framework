using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Shouldly;
using Sprite.Auditing;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Sprite.Audting.Tests
{
    public class CommonTests
    {
        private readonly ITestOutputHelper _testOutput;

        public CommonTests(ITestOutputHelper testOutputHelper)
        {
            _testOutput = testOutputHelper;
        }

        [Fact]
        public void AuditLog_Test()
        {
            var auditConfigBuilder = new AuditConfigBuilder()
                .EnrichWithProperty("UserId", Guid.NewGuid());
            var auditingOptions = auditConfigBuilder.Build();

            var auditLogEntry = new AuditLogEntry()
            {
                BrowserInfo = "Windows Edge v10",
                ClientIpAddress = "127.0.0.1",
                ExecutionTime = DateTime.Now,
                Comments = { "ad", "bc" }
            };
            // auditLogEntry.WithProperty("UserId2", Guid.NewGuid());
            var ad = new AuditLogEntry("comment1", "comment2");

            var comments = auditLogEntry.Comments;
            // auditLogEntry.EnabledComments();
            // auditLogEntry.GetComments()
            foreach (var enricher in auditingOptions.Enrichers)
            {
                enricher.Enrich(auditLogEntry);
            }

            var extraProperties = auditLogEntry.ExtraProperties;
            var adComments = ad.Comments;
        }

        [Fact]
        public void AmbientAuditScope_Test()
        {
            var userId = Guid.NewGuid();
            var auditConfigBuilder = new AuditConfigBuilder()
                .EnrichWithProperty("UserId", userId).EnrichWithProperty("UserName","Foo");
            var auditingOptions = auditConfigBuilder.Build();

            var adtMgr = new AuditingManager(new OptionsWrapper<AuditConfigOptions>(auditingOptions));

            using (var auditScope = adtMgr.Begin())
            {
                adtMgr.Current.ShouldBe(auditScope);

                using (var scope2 = adtMgr.Begin())
                {
                    adtMgr.Current.ShouldBe(scope2);
                }
            }
        }

        [Fact]
        public void AuditStore_Test()
        {
            IAuditStore auditStore = new SimpleLogAuditStore();
            var options = new AuditConfigBuilder().EnrichWithProperty("UserId",Guid.NewGuid()).Build();
            var auditingManager = new AuditingManager(new OptionsWrapper<AuditConfigOptions>(options));
            using (var auditScope = auditingManager.Begin())
            {
                // auditStore.Save(auditScope.Log);
                var property = auditScope.Log.ExtraProperties["UserId"];
                _testOutput.WriteLine(auditScope.Log.ToJsonString());
            }
        }
    }
}