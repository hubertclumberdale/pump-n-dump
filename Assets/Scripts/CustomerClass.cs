using UnityEngine;

public class CustomerClass : MonoBehaviour
{
    public string customerName;
    public CompanyType company;
    private int companyValue;

    public void Initialize(CustomerScriptable data)
    {
        customerName = data.customerName;
        company = data.company;
    }

    public void ApplyEffect(int value)
    {
        companyValue += value;
        Debug.Log($"{customerName} from {company} now has {companyValue}");
    }
}