//===========================================================================
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
    \author    Francois Conti
    \version   1.0.0
*/
//===========================================================================

//---------------------------------------------------------------------------
#include "chai3d.h"
//---------------------------------------------------------------------------
#include <GLFW/glfw3.h>
//---------------------------------------------------------------------------
using namespace chai3d;
using namespace std;
//---------------------------------------------------------------------------
#include "CBullet.h"
//---------------------------------------------------------------------------

//---------------------------------------------------------------------------
// GENERAL SETTINGS
//---------------------------------------------------------------------------

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


//---------------------------------------------------------------------------
// BULLET MODULE VARIABLES
//---------------------------------------------------------------------------

// bullet world
cBulletWorld* bulletWorld;

// bullet objects
cBulletMesh* bulletRing0;
cBulletMesh* bulletRing1;
cBulletMesh* bulletCylinder0;
cBulletMesh* bulletCylinder1;

// bullet static walls and ground
cBulletStaticPlane* bulletInvisibleWall1;
cBulletStaticPlane* bulletInvisibleWall2;
cBulletStaticPlane* bulletInvisibleWall3;
cBulletStaticPlane* bulletInvisibleWall4;
cBulletStaticPlane* bulletInvisibleWall5;
cBulletStaticPlane* bulletGround;

// a pointer the ODE object grasped by the tool
cBulletMesh* graspObject;

// grasp position is respect to object
cVector3d graspPos;

// is grasp currently active?
bool graspActive = false;

// a small line used to display a grasp
cShapeLine* graspLine;


//---------------------------------------------------------------------------
// CHAI3D VARIABLES
//---------------------------------------------------------------------------

// a camera to render the world in the window display
cCamera* camera;

// a viewport to display the scene viewed by the camera
cViewport* viewport = nullptr;

// a light source to illuminate the objects in the world
cSpotLight *light;

// a haptic device handler
cHapticDeviceHandler* handler;

// a virtual tool representing the haptic device in the scene
cGenericTool* tool;

// a label to display the rates [Hz] at which the simulation is running
cLabel* labelRates;


//---------------------------------------------------------------------------
// GENERAL VARIABLES
//---------------------------------------------------------------------------

// flag to indicate if the haptic simulation currently running
bool simulationRunning = false;

// flag to indicate if the haptic simulation has terminated
bool simulationFinished = true;

// a frequency counter to measure the simulation graphic rate
cFrequencyCounter freqCounterGraphics;

// a frequency counter to measure the simulation haptic rate
cFrequencyCounter freqCounterHaptics;

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


//---------------------------------------------------------------------------
// DECLARED FUNCTIONS
//---------------------------------------------------------------------------

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

// this function renders the scene
void renderGraphics(void);

// this function contains the main haptics simulation loop
void renderHaptics(void);

// this function closes the application
void close(void);


//===========================================================================
/*
    DEMO:    03-Bullet-meshes.cpp

    This example illustrates the use of the Bullet framework for simulating
    haptic interaction with dynamic bodies. In this scene we create two 
    mesh gears to which we assign dynamic properties. Haptic interactions
    are computed and forces are applied to the dynamic models.
 */
//===========================================================================

int main(int argc, char* argv[])
{
    //-----------------------------------------------------------------------
    // INITIALIZATION
    //-----------------------------------------------------------------------

    cout << endl;
    cout << "-----------------------------------" << endl;
    cout << "CHAI3D" << endl;
    cout << "Demo: 03-Bullet-mesh" << endl;
    cout << "Copyright 2003-2024" << endl;
    cout << "-----------------------------------" << endl << endl << endl;
    cout << "Keyboard Options:" << endl << endl;
    cout << "[g] - Enable/Disable gravity" << endl;
    cout << "[f] - Enable/Disable full screen mode" << endl;
    cout << "[m] - Enable/Disable vertical mirroring" << endl;
    cout << "[q] - Exit application" << endl;
    cout << endl << endl;


    //-----------------------------------------------------------------------
    // OPEN GL - WINDOW DISPLAY
    //-----------------------------------------------------------------------

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


    // initialize GLEW library
#ifdef GLEW_VERSION
    if (glewInit() != GLEW_OK)
    {
        cout << "failed to initialize GLEW library" << endl;
        glfwTerminate();
        return 1;
    }
#endif


    //-----------------------------------------------------------------------
    // WORLD - CAMERA - LIGHTING
    //-----------------------------------------------------------------------

    // create a dynamic world.
    bulletWorld = new cBulletWorld();

    // set the background color of the environment
    bulletWorld->m_backgroundColor.setWhite();

    // create a camera and insert it into the virtual world
    camera = new cCamera(bulletWorld);
    bulletWorld->addChild(camera);

    // position and oriente the camera
    camera->set(cVector3d(3.0, 0.0, 0.3),    // camera position (eye)
                cVector3d(0.0, 0.0,-0.5),    // lookat position (target)
                cVector3d(0.0, 0.0, 1.0));   // direction of the "up" vector

    // set the near and far clipping planes of the camera
    // anything in front/behind these clipping planes will not be rendered
    camera->setClippingPlanes(0.01, 10.0);

    // set stereo mode
    camera->setStereoMode(stereoMode);

    // set stereo eye separation and focal length (applies only if stereo is enabled)
    camera->setStereoEyeSeparation(0.02);
    camera->setStereoFocalLength(2.0);

    // set vertical mirrored display mode
    camera->setMirrorVertical(mirroredDisplay);

    // create a light source
    light = new cSpotLight(bulletWorld);

    // attach light to camera
    bulletWorld->addChild(light);

    // enable light source
    light->setEnabled(true);

    // position the light source
    light->setLocalPos( 0, 0, 1.2);

    // define the direction of the light beam
    light->setDir(0,0,-1.0);

    // set uniform concentration level of light 
    light->setSpotExponent(0.0);

    // enable this light source to generate shadows
    light->setShadowMapEnabled(true);

    // set the resolution of the shadow map
    light->m_shadowMap->setQualityLow();
    //light->m_shadowMap->setQualityMedium();

    // set light cone half angle
    light->setCutOffAngleDeg(45);


    //-----------------------------------------------------------------------
    // HAPTIC DEVICES / TOOLS
    //-----------------------------------------------------------------------

    // create a haptic device handler
    handler = new cHapticDeviceHandler();

    // get access to the first available haptic device
    shared_ptr<cGenericHapticDevice> hapticDevice;
    handler->getDevice(hapticDevice, 0);

    // retrieve information about the current haptic device
    cHapticDeviceInfo hapticDeviceInfo = hapticDevice->getSpecifications();

    // emulate button if device has a force gripper
    hapticDevice->setEnableGripperUserSwitch(true);

    // create a 3D tool and add it to the world
    tool = new cToolCursor(bulletWorld);
    bulletWorld->addChild(tool);

    // connect the haptic device to the tool
    tool->setHapticDevice(hapticDevice);

    // initialize tool by connecting to haptic device
    tool->start();

    // map the physical workspace of the haptic device to a larger virtual workspace.
    tool->setWorkspaceRadius(1.3);

    // define a radius for the tool (graphical display)
    double toolRadius = 0.06;
    tool->setRadius(toolRadius, toolRadius);

    // hide the device sphere. only show proxy.
    tool->setShowContactPoints(true, false);

    // enable if objects in the scene are going to rotate of translate
    // or possibly collide against the tool. If the environment
    // is entirely static, you can set this parameter to "false"
    tool->enableDynamicObjects(true);

    // haptic forces are enabled only if small forces are first sent to the device;
    // this mode avoids the force spike that occurs when the application starts when 
    // the tool is located inside an object for instance. 
    tool->setWaitForSmallForce(true);

    // start the haptic tool
    tool->start();


    //--------------------------------------------------------------------------
    // WIDGETS
    //--------------------------------------------------------------------------

    // create a font
    cFontPtr font = NEW_CFONT_CALIBRI_20();

    // create a label to display the haptic and graphic rate of the simulation
    labelRates = new cLabel(font);
    labelRates->m_fontColor.setBlack();
    camera->m_frontLayer->addChild(labelRates);


    //-----------------------------------------------------------------------
    // SETUP BULLET WORLD AND OBJECTS
    //-----------------------------------------------------------------------

    // create a small white line that will be enabled every time the operator
    // grasps an object. The line indicated the connection between the
    // position of the tool and the grasp position on the object
    graspLine = new cShapeLine(cVector3d(0,0,0), cVector3d(0,0,0));
    bulletWorld->addChild(graspLine);
    graspLine->m_colorPointA.setRed();
    graspLine->m_colorPointB.setRed();
    graspLine->setShowEnabled(false);


    //////////////////////////////////////////////////////////////////////////
    // BULLET WORLD
    //////////////////////////////////////////////////////////////////////////

    // read the scale factor between the physical workspace of the haptic
    // device and the virtual workspace defined for the tool
    double workspaceScaleFactor = tool->getWorkspaceScaleFactor();

    // stiffness properties
    double maxStiffness	= hapticDeviceInfo.m_maxLinearStiffness / workspaceScaleFactor;

    // set some gravity
    bulletWorld->setGravity(0.0, 0.0,-9.8);


    //////////////////////////////////////////////////////////////////////////
    // 3 BULLET SPHERES
    //////////////////////////////////////////////////////////////////////////

    // create three objects that are added to the world
    bulletRing0 = new cBulletMesh(bulletWorld);
    bulletWorld->addChild(bulletRing0);

    bulletRing1 = new cBulletMesh(bulletWorld);
    bulletWorld->addChild(bulletRing1);

    bulletCylinder0 = new cBulletMesh(bulletWorld);
    bulletWorld->addChild(bulletCylinder0);

    bulletCylinder1 = new cBulletMesh(bulletWorld);
    bulletWorld->addChild(bulletCylinder1);

    // create objects
    cCreateCylinder(bulletCylinder0, 0.6, 0.1, 16, 1, 1, true, true, cVector3d(0.0, 0.0,-0.3));
    cCreateCylinder(bulletCylinder1, 0.6, 0.1, 16, 1, 1, true, true, cVector3d(0.0, 0.0,-0.3));
    cCreateRing(bulletRing0, 0.1, 0.4, 16, 16);
    cCreateRing(bulletRing1, 0.1, 0.4, 16, 16);

    // define some material properties for the gears
    cMaterial matGear;
    matGear.setStiffness(0.5 * maxStiffness);
    matGear.setDynamicFriction(0.7);
    matGear.setStaticFriction(1.0);
    matGear.setBlueCornflower();
    bulletRing0->setMaterial(matGear, true);
    bulletRing1->setMaterial(matGear, true);

    // define some material properties for the axes
    cMaterial matAxis;
    matAxis.setStiffness(0.5 * maxStiffness);
    matAxis.setDynamicFriction(0.7);
    matAxis.setStaticFriction(1.0);
    matAxis.setGrayDark();
    bulletCylinder0->setMaterial(matAxis, true);
    bulletCylinder1->setMaterial(matAxis, true);

    // define some mass properties for each gear
    bulletRing0->setMass(0.05);
    bulletRing1->setMass(0.05);
    bulletCylinder0->setMass(0.05);
    bulletCylinder1->setMass(0.05);

    // create contact model
    bulletRing0->buildContactTriangles(0.001);
    bulletRing1->buildContactTriangles(0.001);
    bulletCylinder0->buildContactTriangles(0.001);
    bulletCylinder1->buildContactTriangles(0.001);

    // estimate their inertia properties
    bulletRing0->estimateInertia();
    bulletRing1->estimateInertia();
    bulletCylinder0->estimateInertia();
    bulletCylinder1->estimateInertia();

    // create dynamic models
    bulletRing0->buildDynamicModel();
    bulletRing1->buildDynamicModel();
    bulletCylinder0->buildDynamicModel();
    bulletCylinder1->buildDynamicModel();

    // create collision detector for haptic interaction
    bulletRing0->createAABBCollisionDetector(toolRadius);
    bulletRing1->createAABBCollisionDetector(toolRadius);
    bulletCylinder0->createAABBCollisionDetector(toolRadius);
    bulletCylinder1->createAABBCollisionDetector(toolRadius);

    // set initial position of each sphere
    bulletRing0->setLocalPos(0.0,-0.6, 0.0);
    bulletRing1->setLocalPos(0.0, 0.6, 0.0);
    bulletCylinder0->setLocalPos(0.0,-0.6, 0.0);
    bulletCylinder1->setLocalPos(0.0, 0.6, 0.0);


    //////////////////////////////////////////////////////////////////////////
    // INVISIBLE WALLS
    //////////////////////////////////////////////////////////////////////////

    // we create 5 static walls to contain the dynamic objects within a limited workspace
    double planeWidth = 1.0;
    bulletInvisibleWall1 = new cBulletStaticPlane(bulletWorld, cVector3d(0.0, 0.0, -1.0), -2.0 * planeWidth);
    bulletInvisibleWall2 = new cBulletStaticPlane(bulletWorld, cVector3d(0.0, -1.0, 0.0), -planeWidth);
    bulletInvisibleWall3 = new cBulletStaticPlane(bulletWorld, cVector3d(0.0, 1.0, 0.0), -planeWidth);
    bulletInvisibleWall4 = new cBulletStaticPlane(bulletWorld, cVector3d(-1.0, 0.0, 0.0), -planeWidth);
    bulletInvisibleWall5 = new cBulletStaticPlane(bulletWorld, cVector3d(1.0, 0.0, 0.0), -0.8 * planeWidth);


    //////////////////////////////////////////////////////////////////////////
    // GROUND
    //////////////////////////////////////////////////////////////////////////

    // create ground plane
    bulletGround = new cBulletStaticPlane(bulletWorld, cVector3d(0.0, 0.0, 1.0), -planeWidth);

    // add plane to world as we will want to make it visibe
    bulletWorld->addChild(bulletGround);

    // create a mesh plane where the static plane is located
    cCreatePlane(bulletGround, 3.0, 3.0, bulletGround->getPlaneConstant() * bulletGround->getPlaneNormal());

    // define some material properties and apply to mesh
    cMaterial matGround;
    matGround.setStiffness(0.3 * maxStiffness);
    matGround.setDynamicFriction(0.2);
    matGround.setStaticFriction(0.0);
    matGround.setWhite();
    matGround.m_emission.setGrayLevel(0.3);
    bulletGround->setMaterial(matGround);

    // setup collision detector for haptic interaction
    bulletGround->createAABBCollisionDetector(toolRadius);


    //--------------------------------------------------------------------------
    // VIEWPORT DISPLAY
    //--------------------------------------------------------------------------

    // get content scale factor
    float contentScaleW, contentScaleH;
    glfwGetWindowContentScale(window, &contentScaleW, &contentScaleH);

    // create a viewport to display the scene.
    viewport = new cViewport(camera, contentScaleW, contentScaleH);


    //-----------------------------------------------------------------------
    // START HAPTIC SIMULATION THREAD
    //-----------------------------------------------------------------------

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

//---------------------------------------------------------------------------

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

    // option - enable/disable gravity
    else if (a_key == GLFW_KEY_G)
    {
        if (bulletWorld->getGravity().length() > 0.0)
        {
            bulletWorld->setGravity(0.0, 0.0, 0.0);
        }
        else
        {
            bulletWorld->setGravity(0.0, 0.0, -9.8);
        }
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

//---------------------------------------------------------------------------

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
    delete bulletWorld;
    delete handler;
}

//---------------------------------------------------------------------------

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


    /////////////////////////////////////////////////////////////////////
    // RENDER SCENE
    /////////////////////////////////////////////////////////////////////

    // update shadow maps (if any)
    bulletWorld->updateShadowMaps(false, mirroredDisplay);

    // render world
    viewport->renderView(framebufferW, framebufferH);

    // wait until all GL commands are completed
    glFinish();

    // check for any OpenGL errors
    GLenum error = glGetError();
    if (error != GL_NO_ERROR) printf("Error:  %s\n", gluErrorString(error));

    // swap buffers
    glfwSwapBuffers(window);

    // signal frequency counter
    freqCounterGraphics.signal(1);
}

//---------------------------------------------------------------------------

void renderHaptics(void)
{
    // simulation in now running
    simulationRunning  = true;
    simulationFinished = false;

    // reset clock
    cPrecisionClock clock;
    clock.reset();

    // main haptic simulation loop
    while(simulationRunning)
    {
        /////////////////////////////////////////////////////////////////////
        // SIMULATION TIME    
        /////////////////////////////////////////////////////////////////////

        // stop the simulation clock
        clock.stop();

        // read the time increment in seconds
        double timeInterval = cClamp(clock.getCurrentTimeSeconds(), 0.0001, 0.001);

        // restart the simulation clock
        clock.reset();
        clock.start();

        // signal frequency counter
        freqCounterHaptics.signal(1);


        /////////////////////////////////////////////////////////////////////
        // HAPTIC FORCE COMPUTATION
        /////////////////////////////////////////////////////////////////////

        // compute global reference frames for each object
        bulletWorld->computeGlobalPositions(true);

        // update position and orientation of tool
        tool->updateFromDevice();

        // compute interaction forces
        tool->computeInteractionForces();


        /////////////////////////////////////////////////////////////////////
        // DYNAMIC SIMULATION
        /////////////////////////////////////////////////////////////////////

        if (graspActive)
        {
            // check if button pressed
            if (tool->getUserSwitch(0))
            {
                // position grasp point in world coordinates
                //cVector3d posA = graspObject->getLocalTransform() * graspPos;
                cVector3d posA = graspObject->getLocalTransform() * graspPos;

                // position of tool
                cVector3d posB = tool->getHapticPoint(0)->getGlobalPosGoal();
                
                // update line
                graspLine->m_pointA = posA;
                graspLine->m_pointB = posB;
                graspLine->setShowEnabled(true);

                // compute force
                const double Kp = 5.0;
                cVector3d force = Kp * (posB - posA);

                // apply force
                graspObject->addExternalForceAtPoint(force, posA - graspObject->getLocalPos());

                // apply reaction force to haptic device
                tool->addDeviceGlobalForce(-force);
            }
            else
            {
                graspLine->setShowEnabled(false);
                graspActive = false;
                graspObject = NULL;
            }
        }
        else
        {
            // get pointer to next interaction point of tool
            cHapticPoint* interactionPoint = tool->getHapticPoint(0);

            // check primary contact point if available
            if (interactionPoint->getNumCollisionEvents() > 0)
            {
                cCollisionEvent* collisionEvent = interactionPoint->getCollisionEvent(0);

                // given the mesh object we may be touching, we search for its owner which
                // could be the mesh itself or a multi-mesh object. Once the owner found, we
                // look for the parent that will point to the ODE object itself.
                cGenericObject* object = collisionEvent->m_object->getOwner()->getOwner();

                // cast to Bullet object
                cBulletMesh* bulletobject = dynamic_cast<cBulletMesh*>(object);

                // if ODE object, we apply interaction forces
                if (bulletobject != NULL)
                {
                    bulletobject->addExternalForceAtPoint(-interactionPoint->getLastComputedForce(),
                        collisionEvent->m_globalPos - object->getLocalPos());


                    // check if button pressed
                    if (tool->getUserSwitch(0))
                    {
                        graspObject = bulletobject;
                        graspPos = collisionEvent->m_localPos;
                        graspActive = true;
                    }
                }
            }
        }

        // update simulation
        bulletWorld->updateDynamics(timeInterval);

        // send forces to haptic device
        tool->applyToDevice();
    }

    // exit haptics thread
    simulationFinished = true;
}


