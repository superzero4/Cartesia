//==============================================================================
/*
    Software License Agreement (BSD License)
    Copyright (c) 2003-2024, CHAI3D
    (www.chai3d.org)

    All rights reserved.

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions
    are met:

    * Redistributions of source code must retain the above copyright
    notice, this list of conditions and the following disclaimer.

    * Redistributions in binary form must reproduce the above
    copyright notice, this list of conditions and the following
    disclaimer in the documentation and/or other materials provided
    with the distribution.

    * Neither the name of CHAI3D nor the names of its contributors may
    be used to endorse or promote products derived from this software
    without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
    "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
    FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
    COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
    INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
    BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
    LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
    CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
    LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
    ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
    POSSIBILITY OF SUCH DAMAGE. 

    \author    <http://www.chai3d.org>
    \author    Sonny Chan
    \author    Francois Conti
    \version   3.3.0
*/
//==============================================================================

//------------------------------------------------------------------------------
#include "chai3d.h"
//------------------------------------------------------------------------------
#include <GLFW/glfw3.h>
//------------------------------------------------------------------------------
using namespace chai3d;
using namespace std;
//------------------------------------------------------------------------------

//------------------------------------------------------------------------------
// GENERAL SETTINGS
//------------------------------------------------------------------------------

// stereo Mode
/*
    C_STEREO_DISABLED:            Stereo is disabled 
    C_STEREO_ACTIVE:              Active stereo for OpenGL NVDIA QUADRO cards
    C_STEREO_PASSIVE_LEFT_RIGHT:  Passive stereo where L/R images are rendered next to each other
    C_STEREO_PASSIVE_TOP_BOTTOM:  Passive stereo where L/R images are rendered above each other
*/
cStereoMode stereoMode = C_STEREO_DISABLED;

// fullscreen mode
bool fullscreen = false;

// mirrored display
bool mirroredDisplay = false;


//------------------------------------------------------------------------------
// STATES
//------------------------------------------------------------------------------
enum MouseStates
{
    MOUSE_IDLE,
    MOUSE_MOVE_CAMERA
};

enum HapticStates
{
    HAPTIC_IDLE,
    HAPTIC_SELECTION
};


//------------------------------------------------------------------------------
// DECLARED VARIABLES
//------------------------------------------------------------------------------

// a world that contains all objects of the virtual environment
cWorld* world;

// a camera to render the world in the window display
cCamera* camera;

// a viewport to display the scene viewed by the camera
cViewport* viewport = nullptr;

// a light source to illuminate the objects in the world
cDirectionalLight *light;

// a haptic device handler
cHapticDeviceHandler* handler;

// a pointer to the current haptic device
cGenericHapticDevicePtr hapticDevice;

// a virtual tool representing the haptic device in the scene
cToolCursor* tool;

// a sphere to show the projected point on the surface
cShapeSphere* cursor;

// a virtual heart object from a CT scan
cVoxelObject* object;

// color lookup tables for the volume
cImagePtr boneLUT;
cImagePtr softLUT;

// angular velocity of object
cVector3d angVel(0.0, 0.0, 0.1);

// a colored background
cBackground* background;

// a font for rendering text
cFontPtr font;

// a label to display the rate [Hz] at which the simulation is running
cLabel* labelRates;

// a label to explain what is happening
cLabel* labelMessage;

// a flag that indicates if the haptic simulation is currently running
bool simulationRunning = false;

// a flag that indicates if the haptic simulation has terminated
bool simulationFinished = true;

// a frequency counter to measure the simulation graphic rate
cFrequencyCounter freqCounterGraphics;

// a frequency counter to measure the simulation haptic rate
cFrequencyCounter freqCounterHaptics;

// mouse state
MouseStates mouseState = MOUSE_IDLE;

// last mouse position
double mouseX, mouseY;

// haptic thread
cThread* hapticsThread;

// a handle to window display context
GLFWwindow* window = nullptr;

// current size of GLFW window
int windowW = 0;
int windowH = 0;

// current size of GLFW framebuffer
int framebufferW = 0;
int framebufferH = 0;

// swap interval for the display context (vertical synchronization)
int swapInterval = 1;


//------------------------------------------------------------------------------
// DECLARED FUNCTIONS
//------------------------------------------------------------------------------

// callback when the window is resized
void onWindowSizeCallback(GLFWwindow* a_window, int a_width, int a_height);

// callback when the window framebuffer is resized
void onFrameBufferSizeCallback(GLFWwindow* a_window, int a_width, int a_height);

// callback when an error GLFW occurs
void onErrorCallback(int a_error, const char* a_description);

// callback when a key is pressed
void onKeyCallback(GLFWwindow* a_window, int a_key, int a_scancode, int a_action, int a_mods);

// callback when window content scaling is modified
void onWindowContentScaleCallback(GLFWwindow* a_window, float a_xscale, float a_yscale);

// callback to handle mouse click
void onMouseButtonCallback(GLFWwindow* a_window, int a_button, int a_action, int a_mods);

// callback to handle mouse motion
void onMouseMotionCallback(GLFWwindow* a_window, double a_posX, double a_posY);

// callback to handle mouse scroll
void onMouseScrollCallback(GLFWwindow* a_window, double a_offsetX, double a_offsetY);

// this function renders the scene
void renderGraphics(void);

// this function contains the main haptics simulation loop
void renderHaptics(void);

// this function closes the application
void close(void);


//==============================================================================
/*
    DEMO:    30-voxel-colormap.cpp

    This demonstration illustrates the use of a isosurfaces and colormap on CT 
    imaging data.
*/
//==============================================================================

int main(int argc, char* argv[])
{
    //--------------------------------------------------------------------------
    // INITIALIZATION
    //--------------------------------------------------------------------------

    cout << endl;
    cout << "-----------------------------------" << endl;
    cout << "CHAI3D" << endl;
    cout << "Demo: 30-voxel-colormap" << endl;
    cout << "Copyright 2003-2024" << endl;
    cout << "-----------------------------------" << endl << endl << endl;
    cout << "Keyboard Options:" << endl << endl;
    cout << "[1] - View skin" << endl;
    cout << "[2] - View bones" << endl;
    cout << "[4,5] Adjust slicing along X axis" << endl;
    cout << "[6,7] Adjust slicing along Y axis" << endl;
    cout << "[8,9] Adjust slicing along Z axis" << endl;
    cout << "[l,h] Adjust quality of graphic rendering" << endl;
    cout << "[p] - Polygonize model and save to file (.stl)" << endl;
    cout << "[f] - Enable/Disable full screen mode" << endl;
    cout << "[m] - Enable/Disable vertical mirroring" << endl;
    cout << "[q] - Exit application" << endl;
    cout << endl << endl;

    // get current path
    bool fileload;
    string currentpath = cGetCurrentPath();


    //--------------------------------------------------------------------------
    // OPEN GL - WINDOW DISPLAY
    //--------------------------------------------------------------------------

    // initialize GLFW library
    if (!glfwInit())
    {
        cout << "failed initialization" << endl;
        cSleepMs(1000);
        return 1;
    }

    // set GLFW error callback
    glfwSetErrorCallback(onErrorCallback);

    // compute desired size of window
    const GLFWvidmode* mode = glfwGetVideoMode(glfwGetPrimaryMonitor());
    windowW = 0.8 * mode->height;
    windowH = 0.5 * mode->height;
    int x = 0.5 * (mode->width - windowW);
    int y = 0.5 * (mode->height - windowH);

    // set OpenGL version
    glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 2);
    glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 1);

    // enable double buffering
    glfwWindowHint(GLFW_DOUBLEBUFFER, GLFW_TRUE);

    // set the desired number of samples to use for multisampling
    glfwWindowHint(GLFW_SAMPLES, 4);

    // specify that window should be resized based on monitor content scale
    glfwWindowHint(GLFW_SCALE_TO_MONITOR, GLFW_TRUE);

    // set active stereo mode
    if (stereoMode == C_STEREO_ACTIVE)
    {
        glfwWindowHint(GLFW_STEREO, GL_TRUE);
    }
    else
    {
        glfwWindowHint(GLFW_STEREO, GL_FALSE);
    }

    // create display context
    window = glfwCreateWindow(windowW, windowH, "CHAI3D", NULL, NULL);
    if (!window)
    {
        cout << "failed to create window" << endl;
        cSleepMs(1000);
        glfwTerminate();
        return 1;
    }

    // set GLFW key callback
    glfwSetKeyCallback(window, onKeyCallback);

    // set GLFW window size callback
    glfwSetWindowSizeCallback(window, onWindowSizeCallback);

    // set GLFW framebuffer size callback
    glfwSetFramebufferSizeCallback(window, onFrameBufferSizeCallback);

    // set GLFW window content scaling callback
    glfwSetWindowContentScaleCallback(window, onWindowContentScaleCallback);

    // set GLFW mouse scroll callback
    glfwSetScrollCallback(window, onMouseScrollCallback);

    // set GLFW mouse position callback
    glfwSetCursorPosCallback(window, onMouseMotionCallback);

    // set GLFW mouse button callback
    glfwSetMouseButtonCallback(window, onMouseButtonCallback);

    // get width and height of window
    glfwGetFramebufferSize(window, &framebufferW, &framebufferH);

    // set position of window
    glfwSetWindowPos(window, x, y);

    // set window size
    glfwSetWindowSize(window, windowW, windowH);

    // set GLFW current display context
    glfwMakeContextCurrent(window);

    // set GLFW swap interval for the current display context
    glfwSwapInterval(swapInterval);


#ifdef GLEW_VERSION
    // initialize GLEW library
    if (glewInit() != GLEW_OK)
    {
        cout << "failed to initialize GLEW library" << endl;
        glfwTerminate();
        return 1;
    }
#endif


    //--------------------------------------------------------------------------
    // WORLD - CAMERA - LIGHTING
    //--------------------------------------------------------------------------

    // create a new world.
    world = new cWorld();

    // set the background color of the environment
    world->m_backgroundColor.setBlack();

    // create a camera and insert it into the virtual world
    camera = new cCamera(world);
    world->addChild(camera);

    // define a basis in spherical coordinates for the camera
    camera->setSphericalReferences(cVector3d(0,0,0),    // origin
                                   cVector3d(0,0,1),    // zenith direction
                                   cVector3d(1,0,0));   // azimuth direction

    camera->setSphericalDeg(2.0,    // spherical coordinate radius
                            60,     // spherical coordinate polar angle
                            10);    // spherical coordinate azimuth angle

    // set the near and far clipping planes of the camera
    // anything in front or behind these clipping planes will not be rendered
    camera->setClippingPlanes(0.1, 10.0);

    // set stereo mode
    camera->setStereoMode(stereoMode);

    // set stereo eye separation and focal length (applies only if stereo is enabled)
    camera->setStereoEyeSeparation(0.02);
    camera->setStereoFocalLength(2.0);

    // set vertical mirrored display mode
    camera->setMirrorVertical(mirroredDisplay);

    // create a light source
    light = new cDirectionalLight(world);

    // attach light to camera
    camera->addChild(light);

    // enable light source
    light->setEnabled(true);

    // define the direction of the light beam
    light->setDir(-3.0,-0.5, 0.0);


    //--------------------------------------------------------------------------
    // HAPTIC DEVICES / TOOLS
    //--------------------------------------------------------------------------

    // create a haptic device handler
    handler = new cHapticDeviceHandler();

    // get access to the first available haptic device found
    handler->getDevice(hapticDevice, 0);

    // retrieve information about the current haptic device
    cHapticDeviceInfo hapticDeviceInfo = hapticDevice->getSpecifications();

    // create a tool (cursor) and insert into the world
    tool = new cToolCursor(world);
    world->addChild(tool);

    // connect the haptic device to the virtual tool
    tool->setHapticDevice(hapticDevice);

    // if the haptic device has a gripper, enable it as a user switch
    hapticDevice->setEnableGripperUserSwitch(true);

    // define a radius for the virtual tool (sphere)
    tool->setRadius(0.02);

    // map the physical workspace of the haptic device to a larger virtual workspace.
    tool->setWorkspaceRadius(0.7);

    // oriente tool with camera
    tool->setLocalRot(camera->getLocalRot());

    // haptic forces are enabled only if small forces are first sent to the device;
    // this mode avoids the force spike that occurs when the application starts when 
    // the tool is located inside an object for instance. 
    tool->setWaitForSmallForce(true);

    // start the haptic tool
    tool->start();

    // read the scale factor between the physical workspace of the haptic
    // device and the virtual workspace defined for the tool
    double workspaceScaleFactor = tool->getWorkspaceScaleFactor();

    // stiffness properties
    double maxStiffness = hapticDeviceInfo.m_maxLinearStiffness / workspaceScaleFactor;


    //--------------------------------------------------------------------------
    // CREATE OBJECT
    //--------------------------------------------------------------------------

    // create a volumetric model
    object = new cVoxelObject();

    // add object to world
    world->addChild(object);
    
    // position object
    object->setLocalPos(0.0, 0.0, 0.0);

    // rotate object
    object->rotateExtrinsicEulerAnglesDeg(0, 0, 65, C_EULER_ORDER_XYZ);
    
    // set the dimensions by assigning the position of the min and max corners
    object->m_minCorner.set(-0.5,-0.5,-0.5);
    object->m_maxCorner.set( 0.5, 0.5, 0.5);
    
    // set the texture coordinate at each corner.
    object->m_minTextureCoord.set(0.0, 0.0, 0.0);
    object->m_maxTextureCoord.set(1.0, 1.0, 1.0);

    // set haptic properties
    object->m_material->setStiffness(0.2 * maxStiffness);
    object->m_material->setStaticFriction(0.0);
    object->m_material->setDynamicFriction(0.0);

    // enable materials
    object->setUseMaterial(true);

    // set material
    object->m_material->setWhite();

    // set quality of graphic rendering
    object->setQuality(0.5);

    // set graphic rendering mode 
    //object->setRenderingModeIsosurfaceColorMap();   // medium quality
    object->setRenderingModeDVRColorMap();            // high quality


    //--------------------------------------------------------------------------
    // LOAD VOXEL DATA
    //--------------------------------------------------------------------------

    // create multi image
    cMultiImagePtr image = cMultiImage::create();

    int filesloaded = image->loadFromFiles(currentpath + "../resources/volumes/heart/heart0", "png", 179);
    if (filesloaded == 0) {
        cout << "Error - Failed to load volume data heartXXXX.png." << endl;
        close();
        return -1;
    }

    // create texture
    cTexture3dPtr texture = cTexture3d::create();

    // assign volumetric image to texture
    texture->setImage(image);

    // assign texture to voxel object
    object->setTexture(texture);

    // create texture
    texture = cTexture3d::create();

    // assign volumetric image to texture
    texture->setImage(image);

    // assign texture to voxel object
    object->setTexture(texture);

    // initially select an isosurface corresponding to the bone/heart level
    object->setIsosurfaceValue(0.30);

    // set optical density factor
    object->setOpticalDensity(1.2);


    //--------------------------------------------------------------------------
    // LOAD COLORMAPS
    //--------------------------------------------------------------------------

    boneLUT = cImage::create();
    
    fileload = boneLUT->loadFromFile(currentpath + "../resources/volumes/heart/colormap_bone.png");
    if (!fileload)
    {
        cout << "Error - Failed to load colormap." << endl;
        close();
        return -1;
    }

    softLUT = cImage::create();

    fileload = softLUT->loadFromFile(currentpath + "../resources/volumes/heart/colormap_soft.png");
    if (!fileload)
    {
        cout << "Error - Failed to load colormap." << endl;
        close();
        return -1;
    }

    // tell the voxel object to load the colour look-up table as a texture
    object->m_colorMap->setImage(boneLUT);


    //--------------------------------------------------------------------------
    // WIDGETS
    //--------------------------------------------------------------------------

    // create a font
    font = NEW_CFONT_CALIBRI_20();
    
    // create a label to display the haptic and graphic rate of the simulation
    labelRates = new cLabel(font);
    camera->m_frontLayer->addChild(labelRates);

    // set font color
    labelRates->m_fontColor.setBlack();

    // create a small message
    labelMessage = new cLabel(font);
    labelMessage->m_fontColor.setBlack();
    labelMessage->setText("press keys [1,2] to toggle colormap and keys [4-9] to adjust slicing.");
    camera->m_frontLayer->addChild(labelMessage);

    // create a background
    background = new cBackground();
    camera->m_backLayer->addChild(background);

    // set background properties
    background->setCornerColors(cColorf(1.0f, 1.0f, 1.0f),
                                cColorf(1.0f, 1.0f, 1.0f),
                                cColorf(0.7f, 0.7f, 0.7f),
                                cColorf(0.7f, 0.7f, 0.7f));


    //--------------------------------------------------------------------------
    // VIEWPORT DISPLAY
    //--------------------------------------------------------------------------

    // get content scale factor
    float contentScaleW, contentScaleH;
    glfwGetWindowContentScale(window, &contentScaleW, &contentScaleH);

    // create a viewport to display the scene.
    viewport = new cViewport(camera, contentScaleW, contentScaleH);


    //--------------------------------------------------------------------------
    // START HAPTIC SIMULATION THREAD
    //--------------------------------------------------------------------------

    // create a thread which starts the main haptics rendering loop
    hapticsThread = new cThread();
    hapticsThread->start(renderHaptics, CTHREAD_PRIORITY_HAPTICS);

    // setup callback when application exits
    atexit(close);


    //--------------------------------------------------------------------------
    // MAIN GRAPHIC LOOP
    //--------------------------------------------------------------------------

    // main graphic loop
    while (!glfwWindowShouldClose(window))
    {
        // render graphics
        renderGraphics();

        // process events
        glfwPollEvents();
    }

    // close window
    glfwDestroyWindow(window);

    // terminate GLFW library
    glfwTerminate();

    // exit
    return 0;
}

//------------------------------------------------------------------------------

void onWindowSizeCallback(GLFWwindow* a_window, int a_width, int a_height)
{
    // update window size
    windowW = a_width;
    windowH = a_height;

    // render scene
    renderGraphics();
}

//------------------------------------------------------------------------------

void onFrameBufferSizeCallback(GLFWwindow* a_window, int a_width, int a_height)
{
    // update frame buffer size
    framebufferW = a_width;
    framebufferH = a_height;
}

//------------------------------------------------------------------------------

void onWindowContentScaleCallback(GLFWwindow* a_window, float a_xscale, float a_yscale)
{
    // update window content scale factor
    viewport->setContentScale(a_xscale, a_yscale);
}

//------------------------------------------------------------------------------

void onErrorCallback(int a_error, const char* a_description)
{
    cout << "Error: " << a_description << endl;
}

//------------------------------------------------------------------------------

void onKeyCallback(GLFWwindow* a_window, int a_key, int a_scancode, int a_action, int a_mods)
{
    // filter calls that only include a key press
    if ((a_action != GLFW_PRESS) && (a_action != GLFW_REPEAT))
    {
        return;
    }

    // option - exit
    else if ((a_key == GLFW_KEY_ESCAPE) || (a_key == GLFW_KEY_Q))
    {
        glfwSetWindowShouldClose(a_window, GLFW_TRUE);
    }

    // option - render bone and heart
    else if (a_key == GLFW_KEY_1)
    {
        object->m_colorMap->setImage(boneLUT);
        object->setIsosurfaceValue(0.30);
        cout << "> Isosurface set to " <<  cStr(object->getIsosurfaceValue(), 3) << "                            \r";
    }

    // option - render soft tissue
    else if (a_key == GLFW_KEY_2)
    {
        object->m_colorMap->setImage(softLUT);
        object->setIsosurfaceValue(0.16);
        cout << "> Isosurface set to " <<  cStr(object->getIsosurfaceValue(), 3) << "                            \r";
    }

    // option - reduce size along X axis
    else if (a_key == GLFW_KEY_4)
    {
        double value = cClamp((object->m_maxCorner.x() - 0.005), 0.01, 0.5);
        object->m_maxCorner.x(value);
        object->m_minCorner.x(-value);
        object->m_maxTextureCoord.x(0.5+value);
        object->m_minTextureCoord.x(0.5-value);
        cout << "> Reduce size along X axis.                            \r";
    }

    // option - increase size along X axis
    else if (a_key == GLFW_KEY_5)
    {
        double value = cClamp((object->m_maxCorner.x() + 0.005), 0.01, 0.5);
        object->m_maxCorner.x(value);
        object->m_minCorner.x(-value);
        object->m_maxTextureCoord.x(0.5+value);
        object->m_minTextureCoord.x(0.5-value);
        cout << "> Increase size along X axis.                            \r";
    }

    // option - reduce size along Y axis
    else if (a_key == GLFW_KEY_6)
    {
        double value = cClamp((object->m_maxCorner.y() - 0.005), 0.01, 0.5);
        object->m_maxCorner.y(value);
        object->m_minCorner.y(-value);
        object->m_maxTextureCoord.y(0.5+value);
        object->m_minTextureCoord.y(0.5-value);
        cout << "> Reduce size along Y axis.                            \r";
    }

    // option - increase size along Y axis
    else if (a_key == GLFW_KEY_7)
    {
        double value = cClamp((object->m_maxCorner.y() + 0.005), 0.01, 0.5);
        object->m_maxCorner.y(value);
        object->m_minCorner.y(-value);
        object->m_maxTextureCoord.y(0.5+value);
        object->m_minTextureCoord.y(0.5-value);
        cout << "> Increase size along Y axis.                            \r";
    }

    // option - reduce size along Z axis
    else if (a_key == GLFW_KEY_8)
    {
        double value = cClamp((object->m_maxCorner.z() - 0.005), 0.01, 0.5);
        object->m_maxCorner.z(value);
        object->m_minCorner.z(-value);
        object->m_maxTextureCoord.z(0.5+value);
        object->m_minTextureCoord.z(0.5-value);
        cout << "> Reduce size along Z axis.                            \r";
    }

    // option - increase size along Z axis
    else if (a_key == GLFW_KEY_9)
    {
        double value = cClamp((object->m_maxCorner.z() + 0.005), 0.01, 0.5);
        object->m_maxCorner.z(value);
        object->m_minCorner.z(-value);
        object->m_maxTextureCoord.z(0.5+value);
        object->m_minTextureCoord.z(0.5-value);
        cout << "> Increase size along Z axis.                            \r";
    }
    // option - decrease quality of graphic rendering
    else if (a_key == GLFW_KEY_L)
    {
        double value = object->getQuality();
        object->setQuality(value - 0.1);
        cout << "> Quality set to " << cStr(object->getQuality(), 1) << "                            \r";
    }

    // option - increase quality of graphic rendering
    else if (a_key == GLFW_KEY_H)
    {
        double value = object->getQuality();
        object->setQuality(value + 0.1);
        cout << "> Quality set to " << cStr(object->getQuality(), 1) << "                            \r";
    }

    // option - polygonize model and save to file
    else if (a_key == GLFW_KEY_P)
    {
        cMultiMesh* surface = new cMultiMesh;
        object->polygonize(surface, 0.004, 0.004, 0.010);
        double SCALE = 0.1;
        double METERS_TO_MILLIMETERS = 1000.0;
        surface->scale(SCALE * METERS_TO_MILLIMETERS);
        surface->saveToFile("human.stl");
        cout << "> Volume has been polygonized and saved to disk                            \r";
        delete surface;
    }

    // option - toggle fullscreen
    else if (a_key == GLFW_KEY_F)
    {
        // toggle state variable
        fullscreen = !fullscreen;

        // get handle to monitor
        GLFWmonitor* monitor = glfwGetPrimaryMonitor();

        // get information about monitor
        const GLFWvidmode* mode = glfwGetVideoMode(monitor);

        // set fullscreen or window mode
        if (fullscreen)
        {
            glfwSetWindowMonitor(window, monitor, 0, 0, mode->width, mode->height, mode->refreshRate);
        }
        else
        {
            int w = 0.8 * mode->height;
            int h = 0.5 * mode->height;
            int x = 0.5 * (mode->width - w);
            int y = 0.5 * (mode->height - h);
            glfwSetWindowMonitor(window, NULL, x, y, w, h, mode->refreshRate);
        }

        // set the desired swap interval and number of samples to use for multisampling
        glfwSwapInterval(swapInterval);
        glfwWindowHint(GLFW_SAMPLES, 4);
    }

    // option - toggle vertical mirroring
    else if (a_key == GLFW_KEY_M)
    {
        mirroredDisplay = !mirroredDisplay;
        camera->setMirrorVertical(mirroredDisplay);
    }
}

//------------------------------------------------------------------------------

void onMouseButtonCallback(GLFWwindow* a_window, int a_button, int a_action, int a_mods)
{
    if (a_button == GLFW_MOUSE_BUTTON_LEFT && a_action == GLFW_PRESS)
    {
        // store mouse position
        glfwGetCursorPos(window, &mouseX, &mouseY);

        // update mouse state
        mouseState = MOUSE_MOVE_CAMERA;
    }
    else
    {
        // update mouse state
        mouseState = MOUSE_IDLE;
    }
}

//------------------------------------------------------------------------------

void onMouseMotionCallback(GLFWwindow* a_window, double a_posX, double a_posY)
{
    if (mouseState == MOUSE_MOVE_CAMERA)
    {
        // compute mouse motion
        int dx = a_posX - mouseX;
        int dy = a_posY - mouseY;
        mouseX = a_posX;
        mouseY = a_posY;

        // compute new camera angles
        double azimuthDeg = camera->getSphericalAzimuthDeg() - 0.5 * dx;
        double polarDeg = camera->getSphericalPolarDeg() - 0.5 * dy;

        // assign new angles
        camera->setSphericalAzimuthDeg(azimuthDeg);
        camera->setSphericalPolarDeg(polarDeg);

        // oriente tool with camera
        tool->setLocalRot(camera->getLocalRot());
    }
}

//------------------------------------------------------------------------------

void onMouseScrollCallback(GLFWwindow* a_window, double a_offsetX, double a_offsetY)
{
    double r = camera->getSphericalRadius();
    r = cClamp(r + 0.1 * a_offsetY, 0.5, 3.0);
    camera->setSphericalRadius(r);
}

//------------------------------------------------------------------------------

void close(void)
{
    // stop the simulation
    simulationRunning = false;

    // wait for graphics and haptics loops to terminate
    while (!simulationFinished) { cSleepMs(100); }

    // close haptic device
    tool->stop();

    // delete resources
    delete hapticsThread;
    delete world;
    delete handler;
}

//------------------------------------------------------------------------------

void renderGraphics(void)
{
    // sanity check
    if (viewport == nullptr) { return; }

    /////////////////////////////////////////////////////////////////////
    // UPDATE WIDGETS
    /////////////////////////////////////////////////////////////////////

    // get width and height of CHAI3D internal rendering buffer
    int displayW = viewport->getDisplayWidth();
    int displayH = viewport->getDisplayHeight();

    // update haptic and graphic rate data
    labelRates->setText(cStr(freqCounterGraphics.getFrequency(), 0) + " Hz / " +
        cStr(freqCounterHaptics.getFrequency(), 0) + " Hz");

    // update position of label
    labelRates->setLocalPos((int)(0.5 * (displayW - labelRates->getWidth())), 15);

    // update position of message label
    labelMessage->setLocalPos((int)(0.5 * (displayW - labelMessage->getWidth())), 40);


    /////////////////////////////////////////////////////////////////////
    // RENDER SCENE
    /////////////////////////////////////////////////////////////////////

    // update shadow maps (if any)
    world->updateShadowMaps(false, mirroredDisplay);

    // render world
    viewport->renderView(framebufferW, framebufferH);

    // wait until all GL commands are completed
    glFinish();

    // check for any OpenGL errors
    GLenum error = glGetError();
    if (error != GL_NO_ERROR) cout << "Error: " << gluErrorString(error) << endl;

    // swap buffers
    glfwSwapBuffers(window);

    // signal frequency counter
    freqCounterGraphics.signal(1);
}

//------------------------------------------------------------------------------

void renderHaptics(void)
{
    HapticStates state = HAPTIC_IDLE;
    cGenericObject* selectedObject = NULL;
    cTransform tool_T_object;

    // simulation in now running
    simulationRunning  = true;
    simulationFinished = false;

    // haptic force activation
    bool flagStart = true;
    int counter = 0;

    // main haptic simulation loop
    while(simulationRunning)
    {
        /////////////////////////////////////////////////////////////////////////
        // HAPTIC RENDERING
        /////////////////////////////////////////////////////////////////////////

        // signal frequency counter
        freqCounterHaptics.signal(1);

        // compute global reference frames for each object
        world->computeGlobalPositions(true);

        // update position and orientation of tool
        tool->updateFromDevice();

        // compute interaction forces
        tool->computeInteractionForces();

        // check if device remains stuck inside voxel object
        cVector3d force = tool->getDeviceGlobalForce();
        if (flagStart)
        {
            if (force.length() != 0.0)
            {
                tool->initialize();
                counter = 0;
            }
            else
            {
                counter++;
                if (counter > 10)
                    flagStart = false;
            }
        }
        else
        {
            if (force.length() > 10.0)
            {
                flagStart = true;
            }
        }


        /////////////////////////////////////////////////////////////////////////
        // MANIPULATION
        /////////////////////////////////////////////////////////////////////////

        // compute transformation from world to tool (haptic device)
        cTransform world_T_tool = tool->getDeviceGlobalTransform();

        // get status of user switch
        bool button = tool->getUserSwitch(0);

        //
        // STATE 1:
        // Idle mode - user presses the user switch
        //
        if ((state == HAPTIC_IDLE) && (button == true))
        {
            // check if at least one contact has occurred
            if (tool->m_hapticPoint->getNumCollisionEvents() > 0)
            {
                // get contact event
                cCollisionEvent* collisionEvent = tool->m_hapticPoint->getCollisionEvent(0);

                // get object from contact event
                selectedObject = collisionEvent->m_object;
            }
            else
            {
                selectedObject = object;
            }

            // get transformation from object
            cTransform world_T_object = selectedObject->getGlobalTransform();

            // compute inverse transformation from contact point to object 
            cTransform tool_T_world = world_T_tool;
            tool_T_world.invert();

            // store current transformation tool
            tool_T_object = tool_T_world * world_T_object;

            // update state
            state = HAPTIC_SELECTION;
        }


        //
        // STATE 2:
        // Selection mode - operator maintains user switch enabled and moves object
        //
        else if ((state == HAPTIC_SELECTION) && (button == true))
        {
            // compute new transformation of object in global coordinates
            cTransform world_T_object = world_T_tool * tool_T_object;

            // compute new transformation of object in local coordinates
            cTransform parent_T_world = selectedObject->getParent()->getLocalTransform();
            parent_T_world.invert();
            cTransform parent_T_object = parent_T_world * world_T_object;

            // assign new local transformation to object
            selectedObject->setLocalTransform(parent_T_object);

            // set zero forces when manipulating objects
            tool->setDeviceGlobalForce(0.0, 0.0, 0.0);

            tool->initialize();
        }

        //
        // STATE 3:
        // Finalize Selection mode - operator releases user switch.
        //
        else
        {
            state = HAPTIC_IDLE;
        }


        /////////////////////////////////////////////////////////////////////////
        // FINALIZE
        /////////////////////////////////////////////////////////////////////////

        // send forces to haptic device
        tool->applyToDevice();  
    }
    
    // exit haptics thread
    simulationFinished = true;
}

//------------------------------------------------------------------------------
