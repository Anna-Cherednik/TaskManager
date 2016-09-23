using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager;
using TaskManager.Models;
using TaskManager.Controllers;
using Moq;

namespace TaskManager.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public async void IndexIsNotNull()
        {
            // Arrange
            var user = new LoginViewModel();
            user.Email = "amofialka@mai.ru";
            user.Password = "03081991aZ!";

            AccountController controller = new AccountController();
            ActionResult res = await controller.Login(user, "Home/Index");

            HomeController testController = new HomeController();
            testController.ControllerContext = controller.ControllerContext;

            // Act
            ViewResult result = testController.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async void IndexViewModelNotNull()
        {
            // Arrange
            var user = new LoginViewModel();
            user.Email = "amofialka@mai.ru";
            user.Password = "03081991aZ!";

            AccountController controller = new AccountController();
            ActionResult res = await controller.Login(user, "Home/Index");

            var mock = new Mock<ITaskRepository>();
            mock.Setup(a => a.GetMyTaskList(user.Email)).Returns(new List<TaskModel>());
            HomeController testController = new HomeController(mock.Object);
            testController.ControllerContext = controller.ControllerContext;

            // Act
            ViewResult result = testController.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result.Model);
        }

        [TestMethod]
        public async void CreateViewModelNotNull()
        {
            // Arrange
            var user = new LoginViewModel();
            user.Email = "amofialka@mai.ru";
            user.Password = "03081991aZ!";

            AccountController controller = new AccountController();
            ActionResult res = await controller.Login(user, "Home/Index");

            var mock = new Mock<ITaskRepository>();
            mock.Setup(a => a.GetMyTaskList(user.Email)).Returns(new List<TaskModel>());
            HomeController testController = new HomeController(mock.Object);
            testController.ControllerContext = controller.ControllerContext;

            // Act
            ViewResult result = testController.Create() as ViewResult;

            // Assert
            Assert.AreEqual("Create task", result.ViewBag.Title);
        }        
    }
}
