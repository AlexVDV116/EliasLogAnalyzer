using NUnit.Framework;

namespace UITests;

public class ThemeChangeTests : BaseTest
{
    [Test]
    public void TestThemeChangeToDarkMode()
    {
        // Navigate to the View menu
        var viewMenu = FindUIElement("ViewMenuBarItem");
        viewMenu.Click();

        // Select "Change Theme" subitem
        var changeThemeItem = FindUIElement("ChangeThemeSubItem");
        changeThemeItem.Click();

        // Select "Dark Mode"
        var darkModeItem = FindUIElement("DarkModeMenuItem");
        darkModeItem.Click();

        // Assert - Check if the theme has changed to dark
        Assert.That(IsDarkModeEnabled(), "Dark mode should be enabled.");
    }

    private bool IsDarkModeEnabled()
    {
        // Assuming there is an element that reflects theme change, for example, the background of a status bar
        var statusBar = FindUIElement("FileMenuBarItem");
        var backgroundColor = statusBar.GetCssValue("background-color");

        // Example CSS value for dark mode might be a darker color; this will need to be specific to your application
        return backgroundColor == "#000000"; // Assuming black is the dark mode background
    }
}