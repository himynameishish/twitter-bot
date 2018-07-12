Imports System.IO
Imports System.Net
Imports System.Text
Imports System.String

Public Class Form1

    'getting rid of the damn popups
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        WebBrowser.ScriptErrorsSuppressed = True
    End Sub

    'log in
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles LogInButton.Click
        If WebBrowser.DocumentText.Contains("Enter a topic, @name, or fullname") Then


            MsgBox("You are already logged in!", 0, "IMPORTANT SHIT")

        ElseIf UsernameBox.Text = "" Or PasswordBox.Text = "" Then

            MsgBox("Enter the required information!", 0, "IMPORTANT SHIT")

        Else

            WebBrowser.Document.GetElementById("session[username_or_email]").SetAttribute("value", UsernameBox.Text)
            WebBrowser.Document.GetElementById("session[password]").SetAttribute("value", PasswordBox.Text)
            WebBrowser.Document.GetElementById("commit").InvokeMember("click")

        End If
    End Sub

    'log out
    Private Sub LogOutButton_Click(sender As Object, e As EventArgs) Handles LogOutButton.Click

        If WebBrowser.DocumentText.Contains("Forgot password?") Then


            MsgBox("You aren't logged in yet!", 0, "IMPORTANT SHIT")

        Else

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

        End If
    End Sub

    'delete all tweets
    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click

        If NumberToDelete.Text = "" Then

            MsgBox("Enter the number of tweets to delete!", 0, "IMPORTANT SHIT")

        Else

            TweetsDeletedCounter.Text = 0

            WebBrowser.Navigate("https://mobile.twitter.com/account")

            'waiting for browser to load
            Do While WebBrowser.ReadyState <> WebBrowserReadyState.Complete
                Application.DoEvents()
            Loop






            Do While TweetsDeletedCounter.Text < NumberToDelete.Text


                Dim SourceCode As String = WebBrowser.DocumentText

                Dim index As Integer
                Dim TweetID As String



                'finding the tweet id
                Do

                    If (Not SourceCode Is Nothing) Then
                        If (SourceCode.Contains("data-id=")) Then
                            index = SourceCode.IndexOf("data-id=")

                            TweetID = SourceCode.Substring(index + 9, 19)




                            'eliminates any extra characters
                            Dim str = TweetID
                            Dim i = TweetID.LastIndexOf("""")
                            If i <> -1 Then
                                TweetID = str.Substring(0, i)
                            End If






                            'MsgBox(TweetID, 0, "IMPORTANT SHIT")

                            WebBrowser.Navigate("https://mobile.twitter.com/statuses/" + TweetID + "/delete")

                            'waiting for browser to load
                            Do While WebBrowser.ReadyState <> WebBrowserReadyState.Complete
                                Application.DoEvents()
                            Loop

                            Dim HtmlElementcoll As HtmlElementCollection = WebBrowser.Document.GetElementsByTagName("input")
                            For Each elem As HtmlElement In HtmlElementcoll
                                ' Deleting
                                If elem.GetAttribute("value") = "Delete" Then
                                    elem.InvokeMember("click")
                                    'adding to the counter                           
                                    TweetsDeletedCounter.Text = TweetsDeletedCounter.Text + 1
                                    WebBrowser.Navigate("https://mobile.twitter.com/account")

                                    'waiting for browser to load
                                    Do While WebBrowser.ReadyState <> WebBrowserReadyState.Complete
                                        Application.DoEvents()
                                    Loop

                                End If
                            Next

                            Exit Do



                        End If
                    End If
                Loop Until SourceCode Is Nothing






            Loop

            MsgBox("Finished!", 0, "Congrats!")

        End If
    End Sub

    'stop deleting
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Application.Exit()
        End
    End Sub

    'post tweet
    Private Sub PostTweet_Click(sender As Object, e As EventArgs) Handles PostTweet.Click
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

End Class
