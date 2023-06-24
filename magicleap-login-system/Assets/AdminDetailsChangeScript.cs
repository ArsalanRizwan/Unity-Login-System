using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AdminDetailsChangeScript : MonoBehaviour
{
    string verifyAdminDetailsUrl = "https://enlivened-custody.000webhostapp.com/verify_admin_details.php";
    string updateAdminDetailsUrl = "https://enlivened-custody.000webhostapp.com/update_admin_details.php";
    public InputField currentAdminIdInputField;
    public InputField currentAdminPasswordInputField;
    public InputField newAdminIdInputField;
    public InputField newAdminPasswordInputField;
    public GameObject adminVerify;
    public GameObject changeDetailsScreen;
    public GameObject adminLoginPage;
    public Text adminVerifyErrorText;
    public Text changeDetailsErrorText;

    public void VerifyAdminDetails()
    {
        string currentAdminId = currentAdminIdInputField.text;
        string currentAdminPassword = currentAdminPasswordInputField.text;

        // Perform input validation
        if (string.IsNullOrEmpty(currentAdminId) || string.IsNullOrEmpty(currentAdminPassword))
        {
            // Display error message
            adminVerifyErrorText.text = "Please fill in all the fields";
            return;
        }

        // Check if the Current Admin ID is 7 digits
        if (currentAdminId.Length != 7)
        {
            // Display error message
            adminVerifyErrorText.text = "Invalid Current Admin ID";
            return;
        }

        StartCoroutine(VerifyAdminDetailsRequest(currentAdminId, currentAdminPassword));
    }

    private IEnumerator VerifyAdminDetailsRequest(string currentAdminId, string currentAdminPassword)
    {
        WWWForm form = new WWWForm();
        form.AddField("currentAdminId", currentAdminId);
        form.AddField("currentAdminPassword", currentAdminPassword);

        using (UnityWebRequest www = UnityWebRequest.Post(verifyAdminDetailsUrl, form))
        {
            yield return www.SendWebRequest();

            if (!www.isNetworkError)
            {
                string responseText = www.downloadHandler.text;
                if (!string.IsNullOrEmpty(responseText))
                {
                    // Deserialize the JSON response
                    VerifyAdminDetailsResponse detailsResponse = JsonUtility.FromJson<VerifyAdminDetailsResponse>(responseText);

                    if (detailsResponse != null && !string.IsNullOrEmpty(detailsResponse.error))
                    {
                        // Display the specific error message
                        adminVerifyErrorText.text = detailsResponse.error;
                    }
                    else if (detailsResponse != null && detailsResponse.success)
                    {
                        // Verification successful, show the change details screen
                        changeDetailsScreen.SetActive(true);
                        adminVerify.SetActive(false);
                    }
                    else
                    {
                        // Invalid response or data
                        Debug.Log("Failed to verify admin details: Invalid response");
                    }
                }
                else
                {
                    // Invalid response
                    Debug.Log("Failed to verify admin details: Invalid response");
                }
            }
            else
            {
                // Network error
                Debug.Log("Failed to verify admin details: " + www.error);
            }
        }
    }

    public void UpdateAdminDetails()
    {
        string newAdminId = newAdminIdInputField.text;
        string newAdminPassword = newAdminPasswordInputField.text;

        // Perform input validation
        if (string.IsNullOrEmpty(newAdminId) || string.IsNullOrEmpty(newAdminPassword))
        {
            // Display error message
            changeDetailsErrorText.text = "Please fill in all the fields";
            return;
        }

        // Check if the New Admin ID is 7 digits
        if (newAdminId.Length != 7)
        {
            // Display error message
            changeDetailsErrorText.text = "Invalid New Admin ID";
            return;
        }

        // Retrieve the current admin ID
        string currentAdminId = currentAdminIdInputField.text;

        StartCoroutine(UpdateAdminDetailsRequest(currentAdminId, newAdminId, newAdminPassword));
    }

    private IEnumerator UpdateAdminDetailsRequest(string currentAdminId, string newAdminId, string newAdminPassword)
    {
        WWWForm form = new WWWForm();
        form.AddField("currentAdminId", currentAdminId);
        form.AddField("newAdminId", newAdminId);
        form.AddField("newAdminPassword", newAdminPassword);

        using (UnityWebRequest www = UnityWebRequest.Post(updateAdminDetailsUrl, form))
        {
            yield return www.SendWebRequest();

            if (!www.isNetworkError)
            {
                string responseText = www.downloadHandler.text;
                if (!string.IsNullOrEmpty(responseText))
                {
                    // Deserialize the JSON response
                    UpdateAdminDetailsResponse detailsResponse = JsonUtility.FromJson<UpdateAdminDetailsResponse>(responseText);

                    if (detailsResponse != null && !string.IsNullOrEmpty(detailsResponse.error))
                    {
                        // Display the specific error message
                        changeDetailsErrorText.text = detailsResponse.error;
                    }
                    else if (detailsResponse != null && detailsResponse.success)
                    {
                        Debug.Log("Admin details updated successfully");

                        adminLoginPage.SetActive(true);
                        changeDetailsScreen.SetActive(false);
                        
                        currentAdminIdInputField.text = string.Empty;
                        currentAdminPasswordInputField.text = string.Empty;
                        newAdminIdInputField.text = string.Empty;
                        newAdminPasswordInputField.text = string.Empty;
                        adminVerifyErrorText.text = string.Empty;
                        changeDetailsErrorText.text = string.Empty;
                    }
                    else
                    {
                        // Invalid response or data
                        Debug.Log("Failed to update admin details: Invalid response");
                    }
                }
                else
                {
                    // Invalid response
                    Debug.Log("Failed to update admin details: Invalid response");
                }
            }
            else
            {
                // Network error
                Debug.Log("Failed to update admin details: " + www.error);
            }
        }
    }

    [System.Serializable]
    private class VerifyAdminDetailsResponse
    {
        public bool success;
        public string error;
    }

    [System.Serializable]
    private class UpdateAdminDetailsResponse
    {
        public bool success;
        public string error;
    }
}
