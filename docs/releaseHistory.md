# Release History

*Disclaimer*

All releases in the v0.x.x series are subject to breaking changes from one version to another.  After the release of v1.0.0, this project will be subject to [semantic versioning](http://semver.org/).

## v0.0.27
"Bug Fixes and Enhancements"
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
