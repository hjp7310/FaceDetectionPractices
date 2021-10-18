# This project is for perform face recognition in C#

Dependency - DlibDotNet, OpenCVSharp4  
Webcam must be installed.  
Dlib-model "shape_predictor_68_face_landmarks.dat" must exist where the exe file is located.  

![MainForm](https://user-images.githubusercontent.com/19831773/137769102-acabcea9-88c1-4953-af00-31f4d330cdab.png)

## Eye tracking
![WebCamForm_EyeTrack](https://user-images.githubusercontent.com/19831773/137769111-03916e5b-3da8-4040-9211-04978bfa39de.png)

## Head pose estimation
![WebCamForm_HeadPoseEstimate](https://user-images.githubusercontent.com/19831773/137769106-8ee2e67b-b01b-49c7-b57a-11f56a6c697b.png)

## Face detection
![WebCamForm_HeadCoordinate](https://user-images.githubusercontent.com/19831773/137769110-3ee7f003-98c5-4b4b-877e-4b36daaafa49.png)

## Unit Test
By putting the "img.jpg" file in the unit test project exe file location, functional testing is possible without a webcam.  

![img](https://user-images.githubusercontent.com/19831773/137769082-d02293a2-e301-43dc-b04a-593cf1daff6e.jpg)
![output_HeadPoseEstimate](https://user-images.githubusercontent.com/19831773/137769086-37d49a79-9f96-4b31-9c88-0da21beb4f97.jpg)
![output_FaceCoordinate](https://user-images.githubusercontent.com/19831773/137769088-ebf046e8-1971-444f-970e-e41597bf937a.jpg)
![output_EyeTracking](https://user-images.githubusercontent.com/19831773/137769089-06cc3dab-6644-40cd-8bc3-48502354e2ec.jpg)

## Refernce
shape_predictor_68_face_landmarks.dat - https://github.com/davisking/dlib-models
