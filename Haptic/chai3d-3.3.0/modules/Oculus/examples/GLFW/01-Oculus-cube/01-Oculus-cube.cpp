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
    \author    Francois Conti
    \version   1.0.0
*/
//==============================================================================

//------------------------------------------------------------------------------
#include "chai3d.h"
//------------------------------------------------------------------------------
#include <GLFW/glfw3.h>
//------------------------------------------------------------------------------
#include "COculus.h"
//------------------------------------------------------------------------------
using namespace chai3d;
using namespace std;
//------------------------------------------------------------------------------

//------------------------------------------------------------------------------
// CHAI3D
//------------------------------------------------------------------------------

// a world that contains all objects of the virtual environment
cWorld* world;

// a camera to render the world in the window display
cCamera* camera;

// a light source
cSpotLight *light;

// a haptic device handler
cHapticDeviceHandler* handler;

// a pointer to the current haptic device
cGenericHapticDevicePtr hapticDevice;

// a virtual tool representing the haptic device in the scene
cToolCursor* tool;

// a virtual mesh like object
cMesh* object;

// indicates if the haptic simulation currently running
bool simulationRunning = false;

// indicates if the haptic simulation has terminated
bool simulationFinished = true;

// frequency counter to measure the simulation haptic rate
cFrequencyCounter frequencyCounter;

// haptic thread
cThread *hapticsThread;


//------------------------------------------------------------------------------
// OCULUS RIFT
//------------------------------------------------------------------------------

// oculus device
cOculusDevice* oculusDisplay;


//------------------------------------------------------------------------------
// DECLARED FUNCTIONS
//------------------------------------------------------------------------------

// callback when an error GLFW occurs
void onErrorCallback(int a_error, const char* a_description);

// callback when a key is pressed
void onKeyCallback(GLFWwindow* a_window, int a_key, int a_scancode, int a_action, int a_mods);

// function that closes the application
void close(void);

// main haptics simulation loop
void renderHaptics(void);


//==============================================================================
/*
    DEMO:   01-cube.cpp

    This example illustrates how to build a small mesh cube.
    The applications also presents the use of texture properties by defining
    a texture image and associated texture coordinates for each of the vertices.
    A bump map is also used in combination with a shader to create a more
    realistic rendering of the surface.
*/
//==============================================================================

int main(int argc, char **argv)
{
    //--------------------------------------------------------------------------
    // INITIALIZATION
    //--------------------------------------------------------------------------

    cout << endl;
    cout << "-----------------------------------" << endl;
    cout << "CHAI3D" << endl;
    cout << "Demo: 01-cube" << endl;
    cout << "Copyright 2003-2024" << endl;
    cout << "-----------------------------------" << endl << endl << endl;
    cout << "Keyboard Options:" << endl << endl;
    cout << "[ ] - Recenter view point" << endl;
    cout << "[q] - Exit application" << endl;
    cout << endl << endl;

    // get current path
    bool fileload;
    string currentpath = cGetCurrentPath();


    //--------------------------------------------------------------------------
    // SETUP DISPLAY CONTEXT
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

    // set OpenGL version
    glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 2);
    glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 1);

    // enable double buffering
    glfwWindowHint(GLFW_DOUBLEBUFFER, GLFW_TRUE);

    // set the desired number of samples to use for multisampling
    glfwWindowHint(GLFW_SAMPLES, 4);

    // specify that window should be resized based on monitor content scale
    glfwWindowHint(GLFW_SCALE_TO_MONITOR, GLFW_TRUE);

    // create display context
    GLFWwindow* window = glfwCreateWindow(640, 480, "CHAI3D", NULL, NULL);
    if (!window)
    {
        cout << "failed to create window" << endl;
        cSleepMs(1000);
        glfwTerminate();
        return 1;
    }

    // set key callback
    glfwSetKeyCallback(window, onKeyCallback);

    // set current display context
    glfwMakeContextCurrent(window);

    // sets the swap interval for the current display context
    glfwSwapInterval(0);

#ifdef GLEW_VERSION
    // initialize GLEW library
    if (glewInit() != GLEW_OK)
    {
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
    // the color is defined by its (R,G,B) components.
    world->m_backgroundColor.setWhite();

    // create a camera and insert it into the virtual world
    camera = new cCamera(world);
    world->addChild(camera);

    // position and orient the camera
    camera->set(cVector3d(0.5, 0.0, 0.2),    // camera position (eye)
                cVector3d(0.0, 0.0, 0.2),    // lookat position (target)
                cVector3d(0.0, 0.0, 1.0));   // direction of the "up" vector

    // set the near and far clipping planes of the camera
    // anything in front/behind these clipping planes will not be rendered
    camera->setClippingPlanes(0.01, 20.0);

    // create a light source
    light = new cSpotLight(world);

    // add light to world
    world->addChild(light);

    // enable light source
    light->setEnabled(true);

    // position the light source
    light->setLocalPos(3.5, 2.0, 0.0);

    // define the direction of the light beam
    light->setDir(-3.5, -2.0, 0.0);

    // set light cone half angle
    light->setCutOffAngleDeg(50);


    //--------------------------------------------------------------------------
    // HAPTIC DEVICES / TOOLS
    //--------------------------------------------------------------------------

    // create a haptic device handler
    handler = new cHapticDeviceHandler();

    // get access to the first available haptic device
    handler->getDevice(hapticDevice, 0);

    // retrieve information about the current haptic device
    cHapticDeviceInfo hapticDeviceInfo = hapticDevice->getSpecifications();

    // create a 3D tool and add it to the world
    tool = new cToolCursor(world);
    world->addChild(tool);

    // connect the haptic device to the tool
    tool->setHapticDevice(hapticDevice);

    // define the radius of the tool (sphere)
    double toolRadius = 0.02;

    // define a radius for the tool
    tool->setRadius(toolRadius);

    // enable if objects in the scene are going to rotate of translate
    // or possibly collide against the tool. If the environment
    // is entirely static, you can set this parameter to "false"
    tool->enableDynamicObjects(true);

    // map the physical workspace of the haptic device to a larger virtual workspace.
    tool->setWorkspaceRadius(0.3);

    // haptic forces are enabled only if small forces are first sent to the device;
    // this mode avoids the force spike that occurs when the application starts when 
    // the tool is located inside an object for instance. 
    tool->setWaitForSmallForce(true);

    // start the haptic tool
    tool->start();


    //--------------------------------------------------------------------------
    // CREATE OBJECT
    //--------------------------------------------------------------------------

    // read the scale factor between the physical workspace of the haptic
    // device and the virtual workspace defined for the tool
    double workspaceScaleFactor = tool->getWorkspaceScaleFactor();

    // stiffness properties
    double maxStiffness = hapticDeviceInfo.m_maxLinearStiffness / workspaceScaleFactor;

    // create a virtual mesh
    object = new cMesh();

    // add object to world
    world->addChild(object);

    // set the position of the object at the center of the world
    object->setLocalPos(0.0, 0.0, 0.0);

    // create cube
    cCreateBox(object, 0.2, 0.2, 0.2);

    // create a texture
    cTexture2dPtr texture = cTexture2d::create();

    fileload = texture->loadFromFile(currentpath + "../resources/images/brick-color.png");
    if (!fileload)
    {
        cout << "Error - Texture image failed to load correctly." << endl;
        close();
        return (-1);
    }

    // apply texture to object
    object->setTexture(texture);

    // enable texture rendering 
    object->setUseTexture(true);

    // Since we don't need to see our polygons from both sides, we enable culling.
    object->setUseCulling(true);

    // set material properties to light gray
    object->m_material->setWhite();

    // compute collision detection algorithm
    object->createAABBCollisionDetector(toolRadius);

    // define a default stiffness for the object
    object->m_material->setStiffness(0.3 * maxStiffness);

    // define some static friction
    object->m_material->setStaticFriction(0.2);

    // define some dynamic friction
    object->m_material->setDynamicFriction(0.2);

    // define some texture rendering
    object->m_material->setTextureLevel(0.1);

    // render triangles haptically on front side only
    object->m_material->setHapticTriangleSides(true, false);

    // create a normal texture
    cNormalMapPtr normalMap = cNormalMap::create();

    // load normal map from file
    fileload = normalMap->loadFromFile(currentpath + "../resources/images/brick-normal.png");
    if (!fileload)
    {
        cout << "Error - Texture image failed to load correctly." << endl;
        close();
        return (-1);
    }

    // assign normal map to object
    object->m_normalMap = normalMap;

    // compute surface normals
    object->computeAllNormals();

    // compute tangent vectors
    object->computeBTN();


    //--------------------------------------------------------------------------
    // CREATE SHADERS
    //--------------------------------------------------------------------------

    // create vertex shader
    cShaderPtr vertexShader = cShader::create(C_VERTEX_SHADER);

    // load vertex shader from file
    fileload = vertexShader->loadSourceFile(currentpath + "../resources/shaders/bump.vert");

    // create fragment shader
    cShaderPtr fragmentShader = cShader::create(C_FRAGMENT_SHADER);

    // load fragment shader from file
    fileload = fragmentShader->loadSourceFile(currentpath + "../resources/shaders/bump.frag");

    // create program shader
    cShaderProgramPtr programShader = cShaderProgram::create();

    // assign vertex shader to program shader
    programShader->attachShader(vertexShader);

    // assign fragment shader to program shader
    programShader->attachShader(fragmentShader);

    // assign program shader to object
    object->setShaderProgram(programShader);

    // link program shader
    programShader->linkProgram();

    // set uniforms
    programShader->setUniformi("uColorMap", 0);
    programShader->setUniformi("uShadowMap", 0);
    programShader->setUniformi("uNormalMap", 2);
    programShader->setUniformf("uInvRadius", 0.0f);


    //--------------------------------------------------------------------------
    // CREATE ENVIRONMENT GLOBE
    //--------------------------------------------------------------------------

    // create a virtual mesh
    cMesh* globe = new cMesh();

    // add object to world
    world->addChild(globe);

    // set the position of the object at the center of the world
    globe->setLocalPos(0.0, 0.0, 0.0);

    // create cube
    cCreateSphere(globe, 6.0, 360, 360);
    globe->setUseDisplayList(true);
    globe->deleteCollisionDetector();

    // create a texture
    cTexture2dPtr textureSpace = cTexture2d::create();

    fileload = textureSpace->loadFromFile(currentpath + "../resources/images/infinity.jpg");
    if (!fileload)
    {
        cout << "Error - Texture image failed to load correctly." << endl;
        close();
        return (-1);
    }

    // apply texture to object
    globe->setTexture(textureSpace);

    // enable texture rendering 
    globe->setUseTexture(true);

    // Since we don't need to see our polygons from both sides, we enable culling.
    globe->setUseCulling(false);

    // disable material properties and lighting
    globe->setUseMaterial(false);


    //--------------------------------------------------------------------------
    // SETUP OCULUS DISPLAY
    //--------------------------------------------------------------------------

    // setup oculus display
    oculusDisplay = new cOculusDevice();

    // initialize oculus
    if (!oculusDisplay->initialize(camera, true, 0.5))
    {
        cout << "Error - Oculus display failed to initialize." << endl;
        close();
        return (-1);
    }

    // set window size
    glfwSetWindowSize(window, oculusDisplay->getWindowWidth(), oculusDisplay->getWindowHeight());

    // recenter oculus
    oculusDisplay->recenterPose();


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

    // main graphic rendering loop
    while (!glfwWindowShouldClose(window))
    {
        // render scene to oculus display
        oculusDisplay->render();

        // finalize
        glfwSwapBuffers(window);
        glfwPollEvents();
    }

    // cleanup
    glfwDestroyWindow(window);

    // exit glfw
    glfwTerminate();

    return (0);
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

    // option - spacebar
    else if (a_key == GLFW_KEY_SPACE)
    {
        oculusDisplay->recenterPose();
    }
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
    delete oculusDisplay;
}

//------------------------------------------------------------------------------

void renderHaptics(void)
{
    // angular velocity of object
    cVector3d angVel(0.0, 0.2, 0.3);

    // reset clock
    cPrecisionClock clock;
    clock.reset();

    // simulation in now running
    simulationRunning = true;
    simulationFinished = false;

    // main haptic simulation loop
    while (simulationRunning)
    {
        /////////////////////////////////////////////////////////////////////
        // SIMULATION TIME
        /////////////////////////////////////////////////////////////////////

        // stop the simulation clock
        clock.stop();

        // read the time increment in seconds
        double timeInterval = clock.getCurrentTimeSeconds();

        // restart the simulation clock
        clock.reset();
        clock.start();

        // update frequency counter
        frequencyCounter.signal(1);


        /////////////////////////////////////////////////////////////////////
        // HAPTIC FORCE COMPUTATION
        /////////////////////////////////////////////////////////////////////

        // compute global reference frames for each object
        world->computeGlobalPositions(true);

        // update position and orientation of tool
        tool->updateFromDevice();

        // compute interaction forces
        tool->computeInteractionForces();

        // send forces to haptic device
        tool->applyToDevice();


        /////////////////////////////////////////////////////////////////////
        // DYNAMIC SIMULATION
        /////////////////////////////////////////////////////////////////////

        // some constants
        const double INERTIA = 0.4;
        const double MAX_ANG_VEL = 10.0;
        const double DAMPING = 0.1;

        // get position of cursor in global coordinates
        cVector3d toolPos = tool->getDeviceGlobalPos();

        // get position of object in global coordinates
        cVector3d objectPos = object->getGlobalPos();

        // compute a vector from the center of mass of the object (point of rotation) to the tool
        cVector3d v = cSub(toolPos, objectPos);

        // compute angular acceleration based on the interaction forces
        // between the tool and the object
        cVector3d angAcc(0, 0, 0);
        if (v.length() > 0.0)
        {
            // get the last force applied to the cursor in global coordinates
            // we negate the result to obtain the opposite force that is applied on the
            // object
            cVector3d toolForce = -tool->getDeviceGlobalForce();

            // compute the effective force that contributes to rotating the object.
            cVector3d force = toolForce - cProject(toolForce, v);

            // compute the resulting torque
            cVector3d torque = cMul(v.length(), cCross(cNormalize(v), force));

            // update rotational acceleration
            angAcc = (1.0 / INERTIA) * torque;
        }

        // update rotational velocity
        angVel.add(timeInterval * angAcc);

        // set a threshold on the rotational velocity term
        double vel = angVel.length();
        if (vel > MAX_ANG_VEL)
        {
            angVel.mul(MAX_ANG_VEL / vel);
        }

        // add some damping too
        angVel.mul(1.0 - DAMPING * timeInterval);

        // if user switch is pressed, set velocity to zero
        if (tool->getUserSwitch(0) == 1)
        {
            angVel.zero();
        }

        // compute the next rotation configuration of the object
        if (angVel.length() > C_SMALL)
        {
            object->rotateAboutGlobalAxisRad(cNormalize(angVel), timeInterval * angVel.length());
        }
    }

    // exit haptics thread
    simulationFinished = true;
}

//------------------------------------------------------------------------------
