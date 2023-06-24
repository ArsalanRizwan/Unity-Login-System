using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AdminLoginScript : MonoBehaviour
{
    string adminLoginUrl = "https://enlivened-custody.000webhostapp.com/admin_login.php";
    public InputField adminIdInputField;
    public InputField adminPasswordInputField;
    public Text errorText;
    public GameObject adminLoginPage;
    public GameObject adminSettings;

    public void AdminLogin()
    {
        string adminId = adminIdInputField.text;
        string adminPassword = adminPasswordInputField.text;

        // Perform input validation
        if (string.IsNullOrEmpty(adminId))
        {
            errorText.text = "Please enter the Admin ID";
            return;
        }

        if (adminId.Length != 7)
        {
            errorText.text = "Admin ID must be 7 digits";
            return;
        }

        if (string.IsNullOrEmpty(adminPassword))
        {
            errorText.text = "Please enter the Admin Password";
            return;
        }

        StartCoroutine(AdminLoginRequest(adminId, adminPassword));
    }

    private IEnumerator AdminLoginRequest(string adminId, string adminPassword)
    {
        WWWForm form = new WWWForm();
        form.AddField("adminId", adminId);
        form.AddField("adminPassword", adminPassword);

        using (UnityWebRequest www = UnityWebRequest.Post(adminLoginUrl, form))
        {
            yield return www.SendWebRequest();

            if (!www.isNetworkError)
            {
                string responseText = www.downloadHandler.text;
                if (!string.IsNullOrEmpty(responseText))
                {
                    // Deserialize the JSON response
                    AdminLoginResponse loginResponse = JsonUtility.FromJson<AdminLoginResponse>(responseText);

                    if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.error))
                    {
                        // Display the specific error message
                        errorText.text = loginResponse.error;
                    }
                    else if (loginResponse != null && loginResponse.success)
                    {
                        Debug.Log("Admin logged in successfully");

                        adminSettings.SetActive(true);
                        adminLoginPage.SetActive(false);

                        adminIdInputField.text = string.Empty;
                        adminPasswordInputField.text = string.Empty;
                        errorText.text = string.Empty;
                    }
                    else
                    {
                        // Invalid response or data
                        Debug.Log("Admin login failed: Invalid response");
                    }
                }
                else
                {
                    // Invalid response
                    Debug.Log("Admin login failed: Invalid response");
                }
            }
            else
            {
                // Network error
                Debug.Log("Admin login failed: " + www.error);
            }
        }
    }

    [System.Serializable]
    private class AdminLoginResponse
    {
        public bool success;
        public string error;
    }
}
