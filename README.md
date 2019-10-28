# credentials

[![Build status](https://ci.appveyor.com/api/projects/status/vs331wn4g0qrtgr0?svg=true)](https://ci.appveyor.com/project/alansav/credentials)

This project is designed to help ensure that passwords are handled using the current best practice.

This project produces a .dll which targets .NET Standard 2.0, .NET 4.5 and .NET 4.6. The dll is packaged & published to nuget.org and is named: Credentials.

This project abstracts most of the implementation away from the developer so that passwords can be easily handled in a secure manner.

If you want to read more info about why this is important then please read my blog article titled [Store Passwords Securely(https://alansav.wordpress.com/2018/06/12/store-passwords-securely/)]

## Usage

In your project use nuget to add a reference to credentials:

`Install-Package Credentials`

### User Registration
When a user registers with your account we would run the following:

```
string username = "bob";
string password = "123456";

var credentials = new Savage.Credentials.Credentials(username, password);
var saltAndHashedPassword = credentials.CreateSaltAndHashedPassword();
```

We would then need to store the username "bob" along with the salt and hashedPassword using the properties in the saltAndHashedPassword object
```
var salt = saltAndHashedPassword.Salt; //an array of bytes (length = 16)
var hashedPassword = saltAndHashedPassword.HashedPassword; //an array of bytes (length = 64)
```

We could store these in most database as bytes, or if we prefer we could convert these to a string using base 64 as follows:
```
var saltBase64 = Convert.ToBase64String(salt); // zXoXq8NcGEIvqAPPuO7xjA==
var hashedPasswordBase64 = Convert.ToBase64String(hashedPassword); // 8wiaJrVMxNjKtMwl8iqhlQ4ylO2ta30wFoI8tJG62bJnuQPhZStFQqJzx2U2zuJ4mZg0Bg1ACaXcVQFe8ywnmg==
```

We can also call ToString() to return the salt and hashedPassword in a single string, such as the below:
```
var store = saltAndHashedPassword.ToString()
$rfc2898$y3eYXhWaoWOZ2AVpalMHPQ==$1024$ZFmqzmel+RuufuP60q/cZaN+GZsdH5vccBMLu/gL199LcGz1VgBpC8oaup9ZC4xnummFAyWfTnzOL0R5pv+WWQ==
```


The database table may look like

| username | salt | hashed_password |
| -------- | ---- | --------------- |
| bob | zXoXq8NcGEIvqAPPuO7xjA== | 8wiaJrVMxNjKtMwl8iqhlQ4ylO2ta30wFoI8tJG62bJnuQPhZStFQqJzx2U2zuJ4mZg0Bg1ACaXcVQFe8ywnmg== |

Note: The original password is not stored in the database.

### Authentication
When bob logs in to the website he enters username: bob, password: 123456 so we would use the same process to authenticate that the password entered on login matches the password when the account was created.

We need to hash the password that bob has used when trying to login and then compare the resulting hash with the hashed_password in the database. If the two match then we know the user has entered the correct password.

The first step is to retrieve the salt and hashed password for the user bob from the database and then call the method ComparePassword to calculate the hash for the provided password using the salt and check to see if this matches the hashed password stored in the database, if these match then the user is authenticated:

```
//The below code would be replaced with a call to the database to retrieve the salt and hashedPassword for the username provided
var salt = Convert.FromBase64String("zXoXq8NcGEIvqAPPuO7xjA==");
var hashedPassword = Convert.FromBase64String("8wiaJrVMxNjKtMwl8iqhlQ4ylO2ta30wFoI8tJG62bJnuQPhZStFQqJzx2U2zuJ4mZg0Bg1ACaXcVQFe8ywnmg==");

//Call the static load method of the SaltAndHashedPassword
var saltAndHashedPassword = Savage.Credentials.SaltAndHashedPassword.Load(salt, hashedPassword);

bool authenticated = saltAndHashedPassword.ComparePassword("123456"); // authenticated = true
```

### Change Password

If we want to allow a user to change their password we would typically have a form with fields for their current password and their new password. We would use the same code as detailed in the authentication section. The users current password would be passed to the ComparePassword method and if the result is true we can then proceed to set a new password:

`var saltAndHashedPassword = Savage.Credentials.SaltAndHashedPassword.New("new_password");`

We would then store the value of the properties Salt and HashedPassword in our database for that user:

```
var salt = saltAndHashedPassword.Salt; //an array of bytes (length = 16)
var hashedPassword = saltAndHashedPassword.HashedPassword; //an array of bytes (length = 64)
```

or if we prefer we could store the values as a string:
```
var saltBase64 = Convert.ToBase64String(salt); // 9vG/lVipM704diYwwWs/eQ==
var hashedPasswordBase64 = Convert.ToBase64String(hashedPassword); // yNEM7eRufrFqdsnlCoknTGNZb+3mT9At5Lfj/152IRCLl466h/B/afdNqJmt+zeulUbmkagiZR7pEDYNB1xhww==
```

### Reset Password
Since we are no longer storing the user's password in the database we are not able to email the user's password to them, we can't send them the hash since they cannot do anything with it.
If the user has forgotten their password then the credentials library has the capability to generate tokens which can then be emailed to the user to perform a password reset.

We would have a page which allows the user to enter the email address and then we use the following code to generate a token:

`var token = Savage.Credentials.Token.Create(24); //24 bytes in length`

This object has the following properties available:

`var clearTextToken = token.ClearTextToken //an array of bytes (length = 24);`

The ClearTextToken is what we would sent to the user (possibly as a querystring value (remember to url encode the token) in a link which they can click to complete the next stage of a password reset)
`var clearTextTokenBase64 = Convert.ToBase64String(clearTextToken); // "mO1/7D+Rk+WnfF5UrKzlOWw7DB410hRn"`

We should not store the ClearTextToken in the database becuase this would allow a user with a compromised database to complete the reset password process for any users who have not yet completed the process. We would call the HashToken value and store the value returned in the database:
`var hashedToken = token.HashToken(); //an array of bytes (length = 64)`

or if we prefer we could store the hashed token as a string in the database:
`var hashedTokenBase64 = Convert.ToBase64String(hashedToken); // "LFs8pEoU6GUL7z9mRbT8970SGUWkRNNWYsDYqSe75ObG7Z9m+RCv9ouDrWynp++Uw53Ip2uC5jBfBxBWqhiMrA=="`

We might have a table in our database called reset_password which would contain this row of data:

| id | username | date_issued | hashed_token | date_verified |
| --- | --- | --- | --- | --- |
| 123 | bob | 2016-07-08 08:00:00 | LFs8pEoU6GUL7z9mRbT8970SGUWkRNNWYsDYqSe75ObG7Z9m+RCv9ouDrWynp++Uw53Ip2uC5jBfBxBWqhiMrA== | null |

Note you will want to use Url.Encode function to encode the clear text token in the querystring first becuase base 64 encoded strings can contain characters such as plus and forward slash which have a different meaning when used in a querystring. To Url encode a value we use the following code:

`var urlEncodedclearTextTokenBase64 = System.Net.WebUtility.UrlEncode(clearTextTokenBase64); // "mO1%2F7D%2BRk%2BWnfF5UrKzlOWw7DB410hRn" `

The user would then receive an email with a link containing something like:

http://example.org/complete_reset_password?id=123&token=mO1%2F7D%2BRk%2BWnfF5UrKzlOWw7DB410hRn

The querystring id would reference the id for the password reset and the token is the clear text token encoded to a base64 string. 

#### Completing the reset password process

When the user clicks the link in their email this would take them to a page which reads the 2 querystring values in and then validates if the token is valid:

```
//Retrieve these values from the querystring
var id = 123;
var tokenValue = "mO1/7D+Rk+WnfF5UrKzlOWw7DB410hRn";
var tokenBytes = Convert.FromBase64String(tokenValue);
 
//Retrieve the hashedToken from the database for id=123
var hashedTokenFromDatabase = "LFs8pEoU6GUL7z9mRbT8970SGUWkRNNWYsDYqSe75ObG7Z9m+RCv9ouDrWynp++Uw53Ip2uC5jBfBxBWqhiMrA==";
var hashedTokenFromDatabaseBytes = Convert.FromBase64String(hashedTokenFromDatabase);
  
//Compare the hash for the token with the known hash stored in the database
var token = Savage.Credentials.Token.Load(tokenBytes);
bool match = token.CompareToken(hashedTokenFromDatabaseBytes);

//If successful:
if (match)
{
    var newPassword = "password"; //Retrieve the new password from the form
    var saltAndHashedPassword = Savage.Credentials.SaltAndHashedPassword.New("newPassword");

    //TODO: Store the value for credentials.Salt and credentials.HashedPassword in the database
    //TODO: Update the database to set the date_verified to the current timestamp for reset_password id 123
}
```

If match is true then the next step is to retrieve the new password the user entered on the page and generate a new salt and hashed password. We should also set the date_verified to the current timestamp and add checks to ensure that we reject the reset request if the date_verified value is not null (i.e. it has already been used).

###Summary
No web application should be storing the passwords in clear text, developers should ensure that all passwords are hashed as this will help protect the users and commercial interests of the owner of the web application.
Using the credentials project provides developers with the capability to handle passwords for users in a secure, consistent and repeatable manor. 

## Technical Details

This project makes a few decisions to help to keep the interface simple:

The salt uses [random bytes generator](https://github.com/alansav/random_bytes_generator) to generate 16 random bytes. Each byte is a value between 0 and 255 so the number of possible salt values is: 16^256

The algorithm to generate the bytes is applied 1024 times to generate a key of 256 bytes.

The key is then hashed the algorithm SHA-512.

The token also uses the [random bytes generator](https://github.com/alansav/random_bytes_generator to generate a token with a length specified by the user. The token is hashed using the algorithm SHA-512.
