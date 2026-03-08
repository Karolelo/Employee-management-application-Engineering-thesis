using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Repo.Server.Tests.UserManagmentModule.Services;

[TestClass]
public class CreateTaskTestSelenium
{
    private IWebDriver _driver;
    private WebDriverWait _wait;
    private readonly string USERNAME = "Mroziu";
    private readonly string PASSWORD = "Tlok12";

    [TestInitialize]
    public void SetUp()
    {
        _driver = new ChromeDriver();
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        _driver.Navigate().GoToUrl("http://localhost:4200/login");
        LoginToUserAccount();
        GoToTaskForm();
    }

    [TestCleanup]
    public void TearDown()
    {
        _driver?.Quit();
        _driver?.Dispose();
    }

    [TestMethod]
    public void User_ShouldCreate_WhenWeUseProperData()
    {
        //Arrange
        var taskData = new TaskData
        {
            Title = "Example",
            Description = "Some example task and yea this shit needs to be at least 50 character to test it",
            TaskStartDate = "2026-10-20",
            TaskTime = "10",
            Priority = "low",
            Status = "To-Do"
        };

        //Act
        FillTaskForm(taskData);
        SubmitTaskForm();

        //Assert
        var newTaskTitle = _wait.Until(d =>
            d.FindElement(By.CssSelector("app-task-list > main > ul > li:last-child > div.task-info > h2"))
        );

        newTaskTitle.Text.Should().Be(taskData.Title);
    }

    [TestMethod]
    public void User_ShouldOpenTaskDetails_WhenClickViewIcon()
    {
        //Arrange
        var viewButton = _wait.Until(d =>
            d.FindElement(By.CssSelector("app-task-list > main > ul > li:last-child > div:last-child > button > i.view-icon"))
        );
        
        //Act
        viewButton.Click();
        
        //Assert
        var dialogContent = _wait.Until(d =>
            d.FindElement(By.CssSelector("mat-dialog-container"))
        );

        dialogContent.Should().NotBeNull();
    }

    #region Helper Methods

    private void FillTaskForm(TaskData taskData)
    {
        FillInput("input[formControlName='name']", taskData.Title);
        FillInput("textarea[formControlName='description']", taskData.Description);
        FillInput("input[formControlName='start_Time']", taskData.TaskStartDate);
        FillInput("input[formControlName='estimated_Time']", taskData.TaskTime);
        SelectFromDropdown("priority", taskData.Priority);
        SelectFromDropdown("status", taskData.Status);
    }

    private void FillInput(string selector, string value)
    {
        var element = _wait.Until(d => d.FindElement(By.CssSelector(selector)));
        element.Clear();
        element.SendKeys(value);
    }

    private void SelectFromDropdown(string formControlName, string value)
    {
        var select = _wait.Until(d =>
            d.FindElement(By.CssSelector($"select[formControlName='{formControlName}']"))
        );

        var selectElement = new SelectElement(select);
        selectElement.SelectByValue(value);
    }
    
    private void SubmitTaskForm()
    {
        
        var submitButton = _wait.Until(d =>
            d.FindElement(By.CssSelector("app-task-form button[type='submit']"))
        );
        
        submitButton.Click();
        
        _wait.Until(d =>
            d.FindElements(By.CssSelector("app-task-list > main > ul > li")).Count > 0
        );
    }

    private void LoginToUserAccount()
    {
        if (_driver.Url.Contains("login"))
        {
            var loginTextBox = _driver.FindElement(By.Id("username"));
            var passwordTextBox = _driver.FindElement(By.Id("password"));

            loginTextBox.SendKeys(USERNAME);
            passwordTextBox.SendKeys(PASSWORD);

            passwordTextBox.Submit();

            _wait.Until(d => !d.Url.Contains("login"));
        }
    }

    private void GoToTaskForm()
    {
        _wait.Until(d => !d.Url.Contains("login"));
        var taskLink = _wait.Until(d =>
            d.FindElement(By.CssSelector("a[href='/tasks']"))
        );
        taskLink.Click();

        _wait.Until(d => d.Url.Contains("/tasks"));
    }

    #endregion

    #region Data Classes

    private class TaskData
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string TaskStartDate { get; set; }
        public string TaskTime { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
    }
    
    private IWebElement WaitForElementToBeClickable(string selector)
    {
        return _wait.Until(d =>
        {
            var element = d.FindElement(By.CssSelector(selector));
            return element.Displayed && element.Enabled ? element : null;
        });
    }

    #endregion
}