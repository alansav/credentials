# credentials

This project is a .NET Core application.

This project is designed to help ensure that passwords are handled using the current best practice.

## Why
Industry best practice is for applications to never store passwords in clear text because of the potential problems if the database is compromised. The recommended practice is to calculate the hash from the password using an approved algorithm. The result of the calculation, the hash, is then stored in the database.

**For example:**

A user creates an account on your website and sets his password to: _123456_

Your website needs to store the users password so when they login we can check if the password they enter on the login page matches the password the user specified when they registered their account, so we might be tempted to store this in a database as clear text, i.e. 123456. This presents a few security problems, if your database is accessed by a malicious user (either a hacker on the other side of the world or a disgruntled employee) they will quickly discover the password for all users and could login to your website as any user. Also, a number of users on your website may use the same password on other websites which would put their login to these websites at risk also. 

Storing passwords is not best practice and we can apply algorithms to the password to calculate a "hash". There are many algorithms which we can use and a common one used is SHA-256. If we calculate the hash using the SHA-256 algorithm for the string "123456" we will have:

`8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92`

Hashing algorithms are one-way in that we cannot "decrypt" the hash to get the original string which was used to generate the hash. We can store the hash in the database securely and if the database is compromised then each password is secure.

When the user logs in, we run the password from the login page through the same algorithm to generate a hash of the password entered on the login screen, if this hash is identical to the password stored in the database then we allow the user access.

While this is a big step toward best practice there are still a few security risks:

* Every user which users the same password will have the same hash
* Hackers will have have a script which checks the output from a hash for commonly used passwords.

A way to address these issues is using a "salt", this is to add something into the algorithm to result in a different hash output:

User 1 registers an account and sets their password as 123456, if we simply prefix the ID for the user to the password (i.e. 1#123456) and then pass this into the algorithm we would have a hash of:

`252dbce23d804860f7bfbc833fdcfc855e0247b4c98b7fd9f58eff1337040f56`

User 2 also registers an account and also sets their password as 123456, now we add the ID for the user to the password (i.e. 2#123456) and then the algorithm would return the following hash:

`edd6342a30a6348460838b685a7234d6bd3977c9582228296314d336ee43f35b`

The resulting hash for user 1 is completely different from the hash for user 2 even though only 1 digit was different.

The other benefit of using hashes is the resulting hash is always the same length, so the result from using SHA-256 to hash the password "1" is:

`6b86b273ff34fce19d6b804eff5a3f5747ada4eaa22f1d49c01e52ddb7875b4b`

and the result from hashing "The quick brown fox jumps over the lazy dog" is:

`d7a8fbb307d7809469ca9abcb0082e4f8d5651e46d3cdb762d02d0bf37c9e592`

Both passwords generate a hash which is 64 characters in length.

To further improve security a technique used is to take the result of the calculated hash and to then calculate the hash for that hash and repeat this process multiple times. Each hash would still be the same length, but we would increase the amount of effort a hacker would need to apply to discover the original password.

The whole process of using a salt, generating a hash and running the hash through the algorithm multiple times is detailed in a document referred to as RFC 2898.

This project abstracts most of the implementation away from the developer so that passwords are handled in a secure manner.

## Usage

In your project use nuget to add a reference to your project:

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

The database table may look like

| username | salt | hashed_password |
| -------- | ---- | --------------- |
| bob | zXoXq8NcGEIvqAPPuO7xjA== | 8wiaJrVMxNjKtMwl8iqhlQ4ylO2ta30wFoI8tJG62bJnuQPhZStFQqJzx2U2zuJ4mZg0Bg1ACaXcVQFe8ywnmg== |

### Authentication
When bob logs in to the website he enters username: bob, password: 123456 so we would use the following process to authenticate that the password entered on login matches the password set when registering:

We would retrieve the salt and hashed password for the user bob from the database and then call the method ComparePassword to calculate the hash for the provided password using the salt and check to see if this matches the hashed password stored in the database, if these match then the user is authenticated:

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

The ClearTextToken is what we would sent to the user (possibly as a querystring value in a link which they can click to complete the next stage of a password reset)
`var clearTextTokenBase64 = Convert.ToBase64String(clearTextToken); // r13j4F4NzthuXOICMHLz1UbmVeTS9QC8`

We would not want to store the ClearTextToken in the database so we would call the GetHashedToken value and store that in the database:
`var hashedToken = token.GetHashedToken(); //an array of bytes (length = 64)`

or if we prefer we could store the hashed token as a string in the database:
`var hashedTokenBase64 = Convert.ToBase64String(hashedToken); //ofDSQoCBaHJkZ6jtBj8iVtaUp8+aRWkfGrT2l09AZ+b1NdWqNveNGZkfg8BZPMUeaZfsQZJAnOFkBEJ3+yWcTA==`

We might have a table in our database called reset_password which would contain this row of data:

id | date_issued | hashed_token | date_verified
-- | ----------- | ------------ | -------------
123 | 2016-07-08 08:00:00 | ofDSQoCBaHJkZ6jtBj8iVtaUp8+aRWkfGrT2l09AZ+b1NdWqNveNGZkfg8BZPMUeaZfsQZJAnOFkBEJ3+yWcTA==|null

The user would then receive an email with a link containing something like:

http://example.org?id=123&token=r13j4F4NzthuXOICMHLz1UbmVeTS9QC8

The querystring id would reference the id for the password reset and the token is the clear text token encoded to a base64 string. Note you will want to use Url.Encode function to encode the values in the querystring first.

## Technical Details

This project makes a few decisions to help to keep the interface simple:

The salt uses the [random bytes generator](https://github.com/alansav/random_bytes_generator) to generate 16 random bytes. Each byte is a value between 0 and 255 so the number of possible salt values is: 16^256

The algorithm to generate the bytes is applied 1024 times to generate a key of 256 bytes.

The key is then hashed using the algorithm SHA-512.
