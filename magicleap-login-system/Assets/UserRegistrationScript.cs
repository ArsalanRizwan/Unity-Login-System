using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UserRegistrationScript : MonoBehaviour
{
    string userRegistrationUrl = "https://enlivened-custody.000webhostapp.com/user_registration.php";
    public InputField nameInputField;
    public InputField emailInputField;
    public InputField userIdInputField;
    public InputField passwordInputField;
    public Text errorText;
    public GameObject loginPage;
    public GameObject registerPage;

    public void UserRegistration()
    {
        string name = nameInputField.text;
        string email = emailInputField.text;
        string userId = userIdInputField.text;
        string password = passwordInputField.text;

        // Perform input validation
        if (string.IsNullOrEmpty(name))
        {
            errorText.text = "Please enter your Name";
            return;
        }

        if (string.IsNullOrEmpty(email))
        {
            errorText.text = "Please enter your Email";
            return;
        }

        if (!IsValidEmail(email))
        {
            errorText.text = "Invalid Email";
            return;
        }

        if (string.IsNullOrEmpty(userId))
        {
            errorText.text = "Please enter the User ID";
            return;
        }

        if (userId.Length != 7)
        {
            errorText.text = "User ID must be 7 digits";
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            errorText.text = "Please enter the Password";
            return;
        }

        StartCoroutine(UserRegistrationRequest(name, email, userId, password));
    }

    private IEnumerator UserRegistrationRequest(string name, string email, string userId, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("email", email);
        form.AddField("userId", userId);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(userRegistrationUrl, form))
        {
            yield return www.SendWebRequest();

            if (!www.isNetworkError)
            {
                string responseText = www.downloadHandler.text;
                if (!string.IsNullOrEmpty(responseText))
                {
                    // Deserialize the JSON response
                    RegistrationResponse registrationResponse = JsonUtility.FromJson<RegistrationResponse>(responseText);

                    if (registrationResponse != null && !string.IsNullOrEmpty(registrationResponse.error))
                    {
                        // Display the specific error message
                        errorText.text = registrationResponse.error;
                    }
                    else if (registrationResponse != null && registrationResponse.success)
                    {
                        Debug.Log("User registered successfully");

                        loginPage.SetActive(true);
                        registerPage.SetActive(false);
                        
                        nameInputField.text = string.Empty;
                        emailInputField.text = string.Empty;
                        userIdInputField.text = string.Empty;
                        passwordInputField.text = string.Empty;
                        errorText.text = string.Empty;
                    }
                    else
                    {
                        // Invalid response or data
                        Debug.Log("User registration failed: Invalid response");
                    }
                }
                else
                {
                    // Invalid response
                    Debug.Log("User registration failed: Invalid response");
                }
            }
            else
            {
                // Network error
                Debug.Log("User registration failed: " + www.error);
            }
        }
    }

    private bool IsValidEmail(string email)
    {
        // Perform email validation here
        // You can use regex or any other method to validate the email format
        // Here's a simple email validation using regex pattern
        string emailPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
        return System.Text.RegularExpressions.Regex.IsMatch(email, emailPattern);
    }

    [System.Serializable]
    private class RegistrationResponse
    {
        public bool success;
        public string error;
    }
}
