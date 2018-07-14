Imports System.IO
Imports System.Net
Imports System.Text
Imports System.String
Imports System.Text.RegularExpressions
Imports System.Threading.Thread
Imports System.Timers

Public Class TwitterBot

    'Delay cause Sleep sucks
    Sub Delay(ByVal dblSecs As Double)
        Const OneSec As Double = 1.0# / (1440.0# * 60.0#)
        Dim dblWaitTil As Date
        Now.AddSeconds(OneSec)
        dblWaitTil = Now.AddSeconds(OneSec).AddSeconds(dblSecs)
        Do Until Now > dblWaitTil
            Application.DoEvents() ' Allow windows messages to be processed
        Loop
    End Sub

    'getting rid of the damn popups
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        WebBrowser.ScriptErrorsSuppressed = True
    End Sub


    'Clicking "Log In" Button
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles LogInButton.Click
        If WebBrowser.DocumentText.Contains("Enter a topic, @name, or fullname") Then
            MsgBox("You are already logged in!", 0, "IMPORTANT SHIT")
        ElseIf UsernameBox.Text = "" Or PasswordBox.Text = "" Then
            MsgBox("Enter the required information!", 0, "IMPORTANT SHIT")
        Else
            Call LOGIN()
            Delay(2)
            If WebBrowser.DocumentText.Contains("Enter a topic, @name, or fullname") Then
                ConsoleLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " - " + "Successfully logged in!" + vbCrLf)
                ConsoleLog.SelectionStart = Len(ConsoleLog.Text)
                ConsoleLog.ScrollToCaret()
                ConsoleLog.Select()
            Else
                ConsoleLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " - " + "Wrong Username/Password, try again!" + vbCrLf)
                ConsoleLog.SelectionStart = Len(ConsoleLog.Text)
                ConsoleLog.ScrollToCaret()
                ConsoleLog.Select()
            End If
        End If
    End Sub

    'Clicking "Log Out" Button
    Private Sub LogOutButton_Click(sender As Object, e As EventArgs) Handles LogOutButton.Click
        If WebBrowser.DocumentText.Contains("Forgot password?") Then
            MsgBox("You aren't logged in yet!", 0, "IMPORTANT SHIT")
        Else
            Call LOGOUT()
            ConsoleLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " - " + "Successfully logged out!" + vbCrLf)
            ConsoleLog.SelectionStart = Len(ConsoleLog.Text)
            ConsoleLog.ScrollToCaret()
            ConsoleLog.Select()
        End If
    End Sub

    'Clicking "Delete Tweets" Button
    Private Sub Delete_Tweets(sender As Object, e As EventArgs) Handles DeleteTweets.Click
        If NumberToDelete.Text = "" Then
            MsgBox("Enter the number of tweets to delete!", 0, "IMPORTANT SHIT")
        Else
            Call DELETE()
        End If
    End Sub

    'Clicking "STOP" Button
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Application.Exit()
        End
    End Sub

    'Clicking "Post Tweet" Button
    Private Sub PostTweet_Click(sender As Object, e As EventArgs) Handles PostTweet.Click
        If Tweet.Text = "" Then
            MsgBox("Enter in a tweet!", 0, "IMPORTANT SHIT")
        Else
            Call TWEETING()
            Delay(2)
            If WebBrowser.DocumentText.Contains("Whoops! You already tweeted that…") Then
                Me.Refresh()
            Else
                ConsoleLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " - " + "Tweet posted!" + vbCrLf)
                ConsoleLog.SelectionStart = Len(ConsoleLog.Text)
                ConsoleLog.ScrollToCaret()
                ConsoleLog.Select()
            End If
        End If
    End Sub

    'Clicking "Clear Log" Button
    Private Sub ClearLog_Click(sender As Object, e As EventArgs) Handles ClearLog.Click
        ConsoleLog.Text = ""
    End Sub



    'Delete Function
    Sub DELETE()

        Dim NumToDelete As Integer = NumberToDelete.Text
        Dim TweetsActuallyDeletedInt As Integer = TweetsActuallyDeletedString.Text

        TweetsActuallyDeletedString.Text = 0

        TweetsActuallyDeletedInt = 0

        WebBrowser.Navigate("https://mobile.twitter.com/account")

        'waiting for browser to load
        Do While WebBrowser.ReadyState <> WebBrowserReadyState.Complete
            Application.DoEvents()
        Loop

        'this loops till the counter is reached
        Do While TweetsActuallyDeletedInt < NumToDelete

            Dim SourceCode As String = WebBrowser.DocumentText

            Dim index As Integer
            Dim TweetID As String

            'Delete Loop
            Do

                If (Not SourceCode Is Nothing) Then
                    'Finding the Tweet ID
                    If (SourceCode.Contains("data-id=")) Then
                        index = SourceCode.IndexOf("data-id=")

                        TweetID = SourceCode.Substring(index + 9, 19)

                        'eliminates any extra characters
                        Dim str = TweetID
                        Dim i = TweetID.LastIndexOf("""")
                        If i <> -1 Then
                            TweetID = str.Substring(0, i)
                        End If

                        System.Console.WriteLine("Attempting to delete Tweet ID: " + TweetID)
                        ConsoleLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " - " + "Attempting to delete ID: " + TweetID + vbCrLf)
                        ConsoleLog.SelectionStart = Len(ConsoleLog.Text)
                        ConsoleLog.ScrollToCaret()
                        ConsoleLog.Select()
                        WebBrowser.Navigate("https://mobile.twitter.com/statuses/" + TweetID + "/delete")

                        'waiting for browser to load
                        Do While WebBrowser.ReadyState <> WebBrowserReadyState.Complete
                            Application.DoEvents()
                        Loop

                        'Deleting tweet and adding to counter
                        Dim HtmlElementcoll As HtmlElementCollection = WebBrowser.Document.GetElementsByTagName("input")
                        For Each elem As HtmlElement In HtmlElementcoll
                            If elem.GetAttribute("value") = "Delete" Then
                                elem.InvokeMember("click")
                                System.Console.WriteLine("Deleted Tweet ID: " + TweetID)
                                ConsoleLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " - " + "Deleted ID: " + TweetID + vbCrLf)
                                ConsoleLog.SelectionStart = Len(ConsoleLog.Text)
                                ConsoleLog.ScrollToCaret()
                                ConsoleLog.Select()
                                TweetsActuallyDeletedInt = TweetsActuallyDeletedInt + 1
                                TweetsActuallyDeletedString.Text = TweetsActuallyDeletedString.Text + 1
                                Delay(1)
                            End If
                        Next
                        Exit Do

                        'if no ID is found, reattempt after a certain amount of time
                    ElseIf WebBrowser.DocumentText.Contains("You've made a few too many attempts. Please try again later.") Then
                        System.Console.WriteLine("Too many attempts.. Retrying in 2 minutes.")
                        ConsoleLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " - " + "Too many attempts.. Retrying in 2 minutes." + vbCrLf)
                        ConsoleLog.SelectionStart = Len(ConsoleLog.Text)
                        ConsoleLog.ScrollToCaret()
                        ConsoleLog.Select()
                        Me.Refresh()
                        Delay(120)
                        Exit Do
                    End If

                End If

            Loop Until SourceCode Is Nothing

            'revisit account page before looping
            WebBrowser.Navigate("https://mobile.twitter.com/account")

            Do While WebBrowser.ReadyState <> WebBrowserReadyState.Complete
                Application.DoEvents()
            Loop

        Loop

        'finished
        ConsoleLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " - " + "Done!" + vbCrLf)
        ConsoleLog.SelectionStart = Len(ConsoleLog.Text)
        ConsoleLog.ScrollToCaret()
        ConsoleLog.Select()
        MsgBox("Finished! Successfully deleted " + TweetsActuallyDeletedString.Text + " tweet(s)!", 0, "Congrats!")

    End Sub

    'Tweet Function
    Sub TWEETING()

        WebBrowser.Navigate("https://mobile.twitter.com/compose/tweet")

        Do While WebBrowser.ReadyState <> WebBrowserReadyState.Complete
            Application.DoEvents()
        Loop

        'load the text in the box
        For Each element As HtmlElement In WebBrowser.Document.GetElementsByTagName("textarea")
            If element.GetAttribute("classname") = "tweetbox" Then
                element.SetAttribute("value", Tweet.Text)
            End If
        Next

        Do While WebBrowser.ReadyState <> WebBrowserReadyState.Complete
            Application.DoEvents()
        Loop

        'send Tweet
        Dim HtmlElementcoll2 As HtmlElementCollection = WebBrowser.Document.GetElementsByTagName("input")
        For Each elem As HtmlElement In HtmlElementcoll2

            If elem.GetAttribute("value") = "Tweet" Then
                elem.InvokeMember("click")
            End If
        Next

    End Sub

    'Log In Function
    Sub LOGIN()
        WebBrowser.Navigate("https://mobile.twitter.com/account")
        Do While WebBrowser.ReadyState <> WebBrowserReadyState.Complete
            Application.DoEvents()
        Loop
        WebBrowser.Document.GetElementById("session[username_or_email]").SetAttribute("value", UsernameBox.Text)
        WebBrowser.Document.GetElementById("session[password]").SetAttribute("value", PasswordBox.Text)
        WebBrowser.Document.GetElementById("commit").InvokeMember("click")
    End Sub

    'Log Out Function
    Sub LOGOUT()
        WebBrowser.Navigate("https://mobile.twitter.com/account")
        Do While WebBrowser.ReadyState <> WebBrowserReadyState.Complete
            Application.DoEvents()
        Loop
        Dim HtmlElementcoll As HtmlElementCollection = WebBrowser.Document.GetElementsByTagName("input")
        For Each elem As HtmlElement In HtmlElementcoll
            ' Check the attributtes you want
            If elem.GetAttribute("value") = "Log out" Then
                'Check even the text if you want
                '  If elem.InnerText = "Sign In" Then
                'Invoke your event
                elem.InvokeMember("click")
                'End If
            End If
        Next
    End Sub
End Class
