using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AdminResetUserPasswordScript : MonoBehaviour
{
    string verifyUserIdUrl = "https://enlivened-custody.000webhostapp.com/verify_user_id.php";
    string resetUserPasswordUrl = "https://enlivened-custody.000webhostapp.com/reset_user_password.php";
    public InputField userIdInputField;
    public GameObject verifyUserScreen;
    public GameObject resetPasswordScreen;
    public InputField newPasswordInputField;
    public GameObject adminSettings;
    public Text verifyUserErrorText;
    public Text resetPassErrorText;

    public void VerifyUserId()
    {
        string userId = userIdInputField.text;

        // Perform input validation
        if (string.IsNullOrEmpty(userId))
        {
            // Display error message
            verifyUserErrorText.text = "Please enter a User ID";
            return;
        }

        // Check if the User ID is 7 digits
        if (userId.Length != 7)
        {
            // Display error message
            verifyUserErrorText.text = "Invalid User ID";
            return;
        }

        StartCoroutine(VerifyUserIdRequest(userId));
    }

    private IEnumerator VerifyUserIdRequest(string userId)
    {
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);

        using (UnityWebRequest www = UnityWebRequest.Post(verifyUserIdUrl, form))
        {
            yield return www.SendWebRequest();

            if (!www.isNetworkError)
            {
                string responseText = www.downloadHandler.text;
                if (!string.IsNullOrEmpty(responseText))
                {
                    // Deserialize the JSON response
                    VerifyUserIdResponse userIdResponse = JsonUtility.FromJson<VerifyUserIdResponse>(responseText);

                    if (userIdResponse != null && !string.IsNullOrEmpty(userIdResponse.error))
                    {
                        // Display the specific error message
                        verifyUserErrorText.text = userIdResponse.error;
                    }
                    else if (userIdResponse != null && userIdResponse.success)
                    {
                        // User ID verification successful, show the reset password screen

                        resetPasswordScreen.SetActive(true);
                        verifyUserScreen.SetActive(false);
                    }
                    else
                    {
                        // Invalid response or data
                        Debug.Log("Failed to verify User ID: Invalid response");
                    }
                }
                else
                {
                    // Invalid response
                    Debug.Log("Failed to verify User ID: Invalid response");
                }
            }
            else
            {
                // Network error
                Debug.Log("Failed to verify User ID: " + www.error);
            }
        }
    }

    public void ResetUserPassword()
    {
        string userId = userIdInputField.text;
        string newPassword = newPasswordInputField.text;

        // Perform input validation
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(newPassword))
        {
            // Display error message
            resetPassErrorText.text = "Please fill in all the fields";
            return;
        }

        // Check if the User ID is 7 digits
        if (userId.Length != 7)
        {
            // Display error message
            resetPassErrorText.text = "Invalid User ID";
            return;
        }

        StartCoroutine(ResetUserPasswordRequest(userId, newPassword));
    }

    private IEnumerator ResetUserPasswordRequest(string userId, string newPassword)
    {
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("newPassword", newPassword);

        using (UnityWebRequest www = UnityWebRequest.Post(resetUserPasswordUrl, form))
        {
            yield return www.SendWebRequest();

            if (!www.isNetworkError)
            {
                string responseText = www.downloadHandler.text;
                if (!string.IsNullOrEmpty(responseText))
                {
                    // Deserialize the JSON response
                    ResetUserPasswordResponse resetResponse = JsonUtility.FromJson<ResetUserPasswordResponse>(responseText);

                    if (resetResponse != null && !string.IsNullOrEmpty(resetResponse.error))
                    {
                        // Display the specific error message
                        resetPassErrorText.text = resetResponse.error;
                    }
                    else if (resetResponse != null && resetResponse.success)
                    {
                        // Password reset successful
                        Debug.Log("User password reset successful");

                        adminSettings.SetActive(true);
                        resetPasswordScreen.SetActive(false);

                        // Clear input fields
                        userIdInputField.text = string.Empty;
                        newPasswordInputField.text = string.Empty;
                        
                        resetPassErrorText.text = string.Empty;
                        verifyUserErrorText.text = string.Empty;
                    }
                    else
                    {
                        // Invalid response or data
                        Debug.Log("Failed to reset user password: Invalid response");
                    }
                }
                else
                {
                    // Invalid response
                    Debug.Log("Failed to reset user password: Invalid response");
                }
            }
            else
            {
                // Network error
                Debug.Log("Failed to reset user password: " + www.error);
            }
        }
    }
}

// Response class for VerifyUserId API
[System.Serializable]
public class VerifyUserIdResponse
{
    public bool success;
    public string error;
}

// Response class for ResetUserPassword API
[System.Serializable]
public class ResetUserPasswordResponse
{
    public bool success;
    public string error;
}
