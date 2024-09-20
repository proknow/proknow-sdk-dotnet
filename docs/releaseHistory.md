# Release History

*Disclaimer*

All releases in the v0.x.x series are subject to breaking changes from one version to another.  After the release of v1.0.0, this project will be subject to [semantic versioning](http://semver.org/).

## v0.4.0

*Enhancements*

- Updated `EntitySummary.GetAsync` and `EntityItem.GetSliceDataAsync` methods to call into componentized RT Visualization service.
- Updated docfx package version.

## v0.3.2

*Enhancements*

- Support for .netstandard2.0 and .netstandard2.1.

## v0.3.1

*Enhancements*

- Upgraded 3rd party dependency packages.

## v0.3.0

*Enhancements*

- Updated the `Patients.query` route to reflect changes to the v2.0.1.0 version of the ProKnow API to return all patients.
- Updated the `ProKnow.Role.Permissions` class to remove the `CanDownloadPatientDICOM` boolean property and added the `CanUpdateOrganizations` property.

## v0.2.5

*Enhancements*

- Added ability to set the client name and version in the User-Agent header for outgoing requests.
- Added inner exceptions to ProKnowExceptions, ProKnowCredentialsStatus and ProKnowDomainStatus objects.

## v0.2.4

*Enhancements*

- Update to work with workspace scorecard templates as well as organizational scorecard templates.
- Updated the JSON to return Rx and Rx Scale for custom metrics in relative mode.

## v0.2.3

*Bug Fixes and Enhancements*

- Added ability for Patients query and Collections.Patients query to get all of the results through paging.
- Fix bug when adding patients to a collection that required both patient ID and entity ID when entity should be optional.

## v0.2.1

*Enhancements*

- Add error code to ProknowApi GetCredentialsStatusAsync response

## v0.2.0

*Enhancements*

- Fix breaking areas for Users and Roles modules ahead of the 1.31.0 release of ProKnow

## v0.1.10

*Bug Fixes*

- Replace ProKnowApi GetConnectionStatusAsync() method and returned type ProKnowConnectionStatus (for which IsValid was always true as long as the base URL was valid and ProKnow was up) with:
    - GetDomainStatusAsync() and returned type ProKnowDomainStatus for which IsOk will be true if the ProKnow domain is up and reachable
    - GetCredentialsStatusAsync() and returned type ProKnowCredentialsStatus for which IsValid will be true if the base URL is valid, the credentials (API key) is valid for the base URL, and ProKnow is up and reachable

## v0.1.9

*Enhancements*

- Add Audit class for interacting with audit logging
- Move scorecards from Entities to Patient namespace

## v0.1.8

*Bug Fixes*

- Fix "Invalid document name" error when streaming documents by adding name parameter to Document StreamAsync()
- Fix "Invalid request to stream non-openable document without attachment parameter" error when streaming non-PDF document" by adding attachment query parameter to API call

## v0.1.7

*Enhancements*

- Append ".dcm" to downloaded image files

## v0.1.6

*Bug Fixes*

- Include patient ProKnow ID in UploadFileOverrides

## v0.1.5

*Bug Fixes and Enhancements*

- Fix bug retrieving upload processing results when there are always 200 or more with non-terminal processing results (i.e., uploads that were initiated whose content was never uploaded)
- Add more logging details when uploading

## v0.1.4

*Enhancements*

- Add logging for ProKnowApi and Uploads classes
- Create symbol package (.snupkg) for debugging 

## v0.1.3

*Bug Fixes*

- Improve HTTP request error handling in Requestor
- Correct exception error message thrown by ProKnowApi constructor when directory containing credentials file is not found 

## v0.1.2

*Bug Fixes*

- Upload large files in chunks to avoid HTTP request and asynchronous task timeouts

## v0.1.1

*Bug Fixes and Enhancements*

- Enable user configuration of retry delays for waiting for ProKnow upload processing through the Uploads RetryDelays property
- Set default retry delays to five 200 msec delays followed by 29 1000 msec delays for a total retry delay of 30 sec
- Fix bug where an upload processing result is occasionally missed
- For GetUploadProcessingResultsAsync:
    - Modify the return to be an UploadProcessingResults class object that includes:
        - The list of UploadProcessingResult for each upload
        - A flag indicating whether the retry delays were exhausted
        - The total retry delay
    - Don't throw an exception if any uploads don't reach a terminal state, just include those uploads in the list with the status of "processing"

## v0.1.0

*Enhancements*

- Separate uploading from waiting for ProKnow processing to complete:
    - UploadAsync only uploads to ProKnow, does not wait for processing, and now returns an IList<UploadResult>
    - Uploads class has new GetUploadProcessingResults method to wait for processing of provided IList<UploadResult> and return an IList<UploadProcessingResult>
    - UploadBatch can now be constructed from an IList<UploadProcessingResult>

## v0.0.29

*Bug Fixes and Enhancements*

- Fix error uploading duplicate objects
- Improve upload performance
- When uploading, don't return an UploadBatch unless waiting for processing to complete
- Throw an exception if, even after retrying, there are still unresolved uploads

## v0.0.28

*Bug Fixes*

- Fix UploadAsync bug handling more than 200 upload results from ProKnow
- Don't return UploadBatch from UploadAsync unless waiting for uploads to reach a terminal state

## v0.0.27

*Bug Fixes and Enhancements*
- Fix construction of UploadBatch to only include data from the upload from which it was created
- Add GetStatus method to UploadBatch to get the status for a specified file

## v0.0.26

*Enhancements*
- Add UploadAsync overloads to Uploads class that are more performant by taking a WorkspaceItem rather than a string for the workspace parameter
- Add interfaces IProKnowApi, IWorkspaces, and IUploads so that users of this library can mock the ProKnowApi, Workspaces, and Uploads classes for testing purposes

## v0.0.25

*New Features and Enhancements*
- Add support for Roles and Users

## v0.0.24

*New Features and Enhancements*
- Add GetSliceDataAsync method to DoseItem class to retrieve voxel data (DoseSlice)
- Add DoseData property to DoseItem class to provide dose properties
- Modify GetImageDataAsync method of ImageSetItem class to return UInt16[] instead of byte[]
- Modify Position property of ImageSetItem to be mm instead of 1/1000 mm

## v0.0.23

*New Features and Enhancements*
- Add GetDataAsync method to StructureSetItem class to retrieve contour and point data (StructureSetRoiData)
- Add SaveAsync method to StructureSetRoiData class to save contour and point data

## v0.0.22

*Enhancements*
- Refactor and fix possible threading issues in unit tests
- Replace use of generic ApplicationException with more specific InvalidOperationError

*Bug fixes*
- Ensure that a structure set draft lock renewer timer does not fire after the edit lock has been removed

## v0.0.21

*New Features and Enhancements*
- Add GetImageDataAsync method to ImageSetItem class to retrieve pixel data
- In ImageSetData and Image classes, replace many ExtensionData with concrete properties

## v0.0.20

*Bug fixes*
- Fix issue limiting threads for uploading to ProKnow

## v0.0.19

*New Features and Enhancements*
- Renamed Roi to StructureSetRoiItem
- Added ability to create a structure set
- Added ability to create a region of interest (ROI) with no contours
- Added ability to retrieve and edit structure set information, including ROIs, except for contour data
- Updated README.md to include developer information for Git Large File Storage (LFS) support
