using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UserLoginScript : MonoBehaviour
{
    string userLoginUrl = "https://enlivened-custody.000webhostapp.com/user_login.php";
    public InputField userIdInputField;
    public InputField passwordInputField;
    public Text errorText;
    public GameObject loginPage;
    public GameObject profilePage;
    public Text name;
    public Text id;
    public Text email;


    public void UserLogin()
    {
        string userId = userIdInputField.text;
        string password = passwordInputField.text;

        // Perform input validation
        if (string.IsNullOrEmpty(userId))
        {
            errorText.text = "Please enter the User ID";
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            errorText.text = "Please enter the Password";
            return;
        }

        if (userId.Length != 7)
        {
            errorText.text = "User ID must be 7 digits";
            return;
        }

        StartCoroutine(UserLoginRequest(userId, password));
    }

    private IEnumerator UserLoginRequest(string userId, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(userLoginUrl, form))
        {
            yield return www.SendWebRequest();

            if (!www.isNetworkError)
            {
                string responseText = www.downloadHandler.text;
                if (!string.IsNullOrEmpty(responseText))
                {
                    // Deserialize the JSON response
                    LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(responseText);

                    if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.error))
                    {
                        // Display the specific error message
                        errorText.text = loginResponse.error;
                    }
                    else if (loginResponse != null && loginResponse.success)
                    {
                        Debug.Log("User logged in successfully");

                        // Get user data from the response
                        string userName = loginResponse.name;
                        string userEmail = loginResponse.email;
                        string userUniqueId = loginResponse.uniqueId;

                        // Display user data or pass it to another script/scene for display
                        Debug.Log("User Name: " + userName);
                        Debug.Log("User Email: " + userEmail);
                        Debug.Log("User Unique ID: " + userUniqueId);

                        // Load the ProfilePage
                        profilePage.SetActive(true);
                        loginPage.SetActive(false);
                        name.text = userName;
                        email.text = userEmail;
                        id.text = userUniqueId;

                        userIdInputField.text = string.Empty;
                        passwordInputField.text = string.Empty;
                        errorText.text = string.Empty;
                    }
                    else
                    {
                        // Invalid response or data
                        Debug.Log("User login failed: Invalid response");
                    }
                }
                else
                {
                    // Invalid response
                    Debug.Log("User login failed: Invalid response");
                }
            }
            else
            {
                // Network error
                Debug.Log("User login failed: " + www.error);
            }
        }
    }

    [System.Serializable]
    private class LoginResponse
    {
        public bool success;
        public string error;
        public string name;
        public string email;
        public string uniqueId;
    }
}