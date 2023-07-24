# ClientCredentialAuthFlow
Practise Client Credential Auth flow in azure.
Api to Api with app roles.

# Azure Setup

## Protected Api
1. Create app registraion for protect resource
2. Set up redirect URL if required.
    - Used when you let the Admin of the protect app consent to your application using the protected resource.
    - Url for consent will look like this where the clientID is the public app registrations id:
		- https://login.microsoftonline.com/{tenantId}/adminconsent?client_id={clientId}&state={somenumber}&redirect_uri={redirect, set to local host for debugging}
3. Create Application Role.
4. Edit manifest.json and update accessTokenAcceptedVersion to 2


## Public Api
1. Create app registraion for public resource
2. In Api Permissions add role from Protect Api setup
    - Add an Application Permission => Select Protect API registraion => select Application Role => select Role
3. Request consent from admin via URL or through the ad portal if on the same tenant.
    - https://login.microsoftonline.com/{tenantId}/adminconsent?client_id={clientId}&state={somenumber}&redirect_uri={redirect, set to local host for debugging}
4. Edit manifest.json and update accessTokenAcceptedVersion to 2.
     - Note might not need this its for the registration issuing tokens with scp

## Check Roles have been granted to the service principal

Go to the protected api service principal (Enterprise app) and check the users and groups.
There should a user for the public app service principal with a role


# Local Settings setup

## Protected API local settings

``` json
"AzureAd": {
  "TenantId": "",
  "ClientId": "",
  "Roles": [ "Role you created" ],
  "ScopeName": "access_as_user",
  "Scope": "api://{protectAPI Client Id}/.default"
},
```
## Public Api Local settings

``` json
"AzureAd": {
  "TenantId": "{public api tenant id}",
  "ClientId": "{not used}",
  "Roles": [ "{not used}" ],
  "ScopeName": "{not used}",
  "Scope": "{not used}"
},
"AzureClientCredential": {
  "ClientId": "{public api client id}",
  "Scope": "api://{protected api client id}/.default",
  "GrantType": "client_credentials",
  "ClientSecret": "{public api client secret}"
}
```


# flow

	1. Assumes consent has been granted by the admin of the protected resource.
	2. Request an access token to the oauth2 token url
		a. https://login.microsoftonline.com/{tenantid}/oauth2/v2.0/token
		b. With payoad
			i. client_id => the clientId of the public api (where the request is coming from)
			ii. Grant_type => client_crednetials
			iii. Scope => the application ID URI of the protected resource with /.default
			iv. Client_secret => the public api client secret.
	3. The returned access_token should have your roles.

