
using System.Threading.Tasks;
using Moq;
using RSOI.Services;
using Xunit;

namespace RSOI.Test.Controllers
{ 
    public class JobControllerTest
    {
        [Fact]
        public async Task Index_Returns200()
        {
            var mockManager = new Mock<IManagerService>();
            mockManager.Setup(mgr => mgr.)
        }
    }
}