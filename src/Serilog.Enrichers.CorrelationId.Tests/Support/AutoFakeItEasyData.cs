using AutoFixture;
using AutoFixture.NUnit3;
using Microsoft.AspNetCore.Http;

namespace Serilog.Tests.Support
{
    public class AutoFakeItEasyDataAttribute : AutoDataAttribute
    {

        public AutoFakeItEasyDataAttribute() : base(
            () => new Fixture()
                .Customize(new AutoFakeItEasyCustomization())
                .Customize(new MvcCustomization())
        )
        {
            
        }
        private class MvcCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {

                fixture.Register(() => new ViewDataDictionary(new EmptyModelMetadataProvider(),
                    new ModelStateDictionary()));

                fixture.Register(() => new ActionContext(A.Dummy<HttpContext>(), new RouteData(), new ActionDescriptor()));

            }
        }
    }
}