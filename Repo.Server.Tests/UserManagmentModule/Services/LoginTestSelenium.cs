using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Repo.Server.Tests.UserManagmentModule.Services;

[TestClass]
public class LoginTestSelenium
{
   private IWebDriver driver;
   
   [TestInitialize]
   public void SetUp()
   {
      driver = new ChromeDriver();
      driver.Navigate().GoToUrl("http://localhost:4200/login");
   }
   
   [TestCleanup]
   public void TearDown()
   {
      driver?.Quit();
      driver?.Dispose();
   }

   [TestMethod]
   public void LoginPage_ShouldDisplayError_WhenWeWriteWrongLoginOrPassword()
   {
      //Arrange
      var usernameTextBox = driver.FindElement(By.Id("username"));
      var passwordTextBox = driver.FindElement(By.Id("password"));
      var submitButton = driver.FindElement(By.CssSelector("button[type=submit]"));
      
      usernameTextBox.SendKeys("Bad");
      passwordTextBox.SendKeys("Bad12");
      
      //I know here that I can use just submit without a button, but it's for learning and readability
      //Act
      submitButton.Click();
      
      //Assert
      var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
      var snackBar = wait.Until(d =>
         d.FindElements(By.CssSelector(".mat-mdc-snack-bar-label"))
            .FirstOrDefault(el => el.Text.StartsWith("Error during login:"))
         );
      
      snackBar.Should().NotBeNull("Snackbar did not display");
      snackBar.Text.Should().Contain("Error during login:"); 
   }

   [TestMethod]
   public void LoginPage_ShouldRedirectUsToMainPanel_WhenWeGiveCorrectLoginData()
   {
      //Arrange
      var usernameTextBox = driver.FindElement(By.Id("username"));
      var passwordTextBox = driver.FindElement(By.Id("password"));
      var submitButton = driver.FindElement(By.CssSelector("button[type=submit]"));

      usernameTextBox.SendKeys("Mroziu");
      passwordTextBox.SendKeys("Tlok12");

      //Act
      submitButton.Click();

      //Assert
      var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
      wait.Until(d => d.Url != "http://localhost:4200/login");
    
      Assert.IsFalse(driver.Url.Contains("/login"), 
         $"Should not be on login page anymore, but URL is: {driver.Url}");
   }
}