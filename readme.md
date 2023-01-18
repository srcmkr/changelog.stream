<a name="readme-top"></a>

[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]
[![LinkedIn][linkedin-shield]][linkedin-url]

<br />
<div align="center">
<h1 align="center">changelog.stream</h1>

  <p align="center">
    Automate your changelog generation for your <a href="https://freshdesk.com" target="_blank">FreshDesk Ticket System</a>.
    <br />
    <a href="https://github.com/srcmkr/changelog.stream/issues">Report Bug</a>
    ·
    <a href="https://github.com/srcmkr/changelog.stream/issues">Request Feature</a>
  </p>
</div>

<!-- ABOUT THE PROJECT -->
## About The Project

[![Screenshot][product-screenshot]](https://github.com/srcmkr/changelog.stream)

changelog.stream is an automated solution for creating a Changelog from support tickets in the software [FreshDesk](https://www.freshdesk.com). 
It is designed to make it easier for both the support team and end-users to keep track of resolved issues and software updates. 
This solution comes in the form of a Docker image, making it easy to integrate into a CI/CD pipeline. 
With this software, generating a Changelog can be done easily and efficiently, saving time and streamlining the process.

Here's why:
* Time-saving: Automating the Changelog creation process with our software saves a significant amount of time compared to manually updating it
* Improved accuracy: The software automatically groups and organizes the tickets based on their attributes, reducing the likelihood of errors or omissions
* Increased efficiency: Integrating the software into a CI/CD pipeline allows for the Changelog to be automatically updated with each release, making the process more efficient and streamlined.

One of the key benefits of using our software is the time-saving aspect. Manually updating a Changelog can be a time-consuming task, especially when it needs to be done regularly. By automating the process, our software eliminates the need for manual labor and allows you to use your time for more important tasks. With the software, the entire process is done quickly and efficiently, without the need for manual intervention, meaning that you can focus on other important tasks. This can be particularly beneficial for organizations with a high volume of support tickets, where manually updating the Changelog would be a daunting task.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- USAGE EXAMPLES -->
## Usage

Here is a full example of how to use the docker image:
````
    docker run -d -p 8080:80 srcmkr/changelogstream \
     -e 'NAME=My Changelog' \
     -e 'TOKEN=some_token' \
     -e 'FD_ENDPOINT=https://myinstance.freshdesk.com' \
     -e 'FD_APITOKEN=abc123' \
     -e 'FD_CLOSEDID=5' \
     -e 'FD_ENTRY=cf_alternative_changelog_entry' \
     -e 'FD_FIXED=cf_fixed_in_version' \
     -e 'FD_TICKETURL=https://myinstance.freshdesk.com/a/tickets'
     -v /path/on/your/host/cl.db:/app/data/cl.db
     -v /path/on/your/host/design.css:/app/data/design.css
````

As part of your CI/CD pipeline, you can call the following URLs via HTTP GET:
#### Create a new release
* ``/build/create/{version}/{token}/{date}`` - Creates a new release entry in the database with date given
* ``/build/create/{version}/{token}`` - Creates a new release entry in the database with current date

#### Create output in different formats
* ``/export/json/`` - Returns a JSON representation of your changelog
* ``/export/html/`` - Returns a formatted HTML page with your changelog
* ``/export/markdown/`` - Returns a formatted markdown page with your changelog

### The following parameters for the docker image are available:

| Variable | Description                                                                                                                             | Required |
| -------- |-----------------------------------------------------------------------------------------------------------------------------------------| -------- |
| NAME | The name / title of the changelog                                                                                                       | Yes |
 | TOKEN | The token used to add or refresh a changelog version (please also consider using a firewall)                                            | Yes |
| FD_ENDPOINT | The FreshDesk endpoint without trailing slash (e.g. https://xxx.freshdesk.com)                                                          | Yes |
| FD_APITOKEN | The FreshDesk API token (Login to FreshDesk, click on avatar in top right corner, click on your profile, API Key is in the upper right) | Yes |
| FD_CLOSEDID | The ID of the status "Closed", default is 5                                                                                             | Yes |
| FD_ENTRY | The name of the custom field that contains the changelog entry, default is "cf_alternative_changelog_entry"                             | No |
| FD_FIXED | The name of the custom field that contains the fixed in version, default is "cf_fixed_in_version"                                       | No |
| FD_TICKETURL | The URL to the ticket, default is "https://xxx.freshdesk.com/a/tickets"                                                                 | No |

### The following two mounts are available:
_It is highly recommended to mount these to a volume on your host_

| Mount | Description                                                                                                          |
| ----- |----------------------------------------------------------------------------------------------------------------------|
| /app/data/cl.db | The database file                                                                                                    |
| /app/data/design.css | Your custom CSS design (see [example](https://github.com/srcmkr/changelog.stream/blob/master/CLApi/data/design.css)) |

Open http://localhost:8080 in your browser to access the API.
<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

_changelog.stream is a private open source project and not affiliated with FreshDesk or FreshWorks Inc. in any way._

<!-- MARKDOWN LINKS & IMAGES -->
[contributors-shield]: https://img.shields.io/github/contributors/srcmkr/changelog.stream.svg?style=for-the-badge
[contributors-url]: https://github.com/srcmkr/changelog.stream/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/srcmkr/changelog.stream.svg?style=for-the-badge
[forks-url]: https://github.com/srcmkr/changelog.stream/network/members
[stars-shield]: https://img.shields.io/github/stars/srcmkr/changelog.stream.svg?style=for-the-badge
[stars-url]: https://github.com/srcmkr/changelog.stream/stargazers
[issues-shield]: https://img.shields.io/github/issues/srcmkr/changelog.stream.svg?style=for-the-badge
[issues-url]: https://github.com/srcmkr/changelog.stream/issues
[license-shield]: https://img.shields.io/github/license/srcmkr/changelog.stream?style=for-the-badge
[license-url]: https://github.com/srcmkr/changelog.stream/blob/master/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://www.linkedin.com/in/srcmkr/
[product-screenshot]: .github/images/screenshot.png