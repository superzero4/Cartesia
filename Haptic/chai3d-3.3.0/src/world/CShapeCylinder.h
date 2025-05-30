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
    \author    Sebastien Grange
    \version   3.3.0
*/
//==============================================================================

//------------------------------------------------------------------------------
#ifndef CShapeCylinderH
#define CShapeCylinderH
//------------------------------------------------------------------------------
#include "world/CGenericObject.h"
#include "materials/CMaterial.h"
#include "materials/CTexture2d.h"
//------------------------------------------------------------------------------
#ifdef C_USE_OPENGL
#ifdef MACOSX
#include "OpenGL/glu.h"
#else
#include "GL/glu.h"
#endif
#endif
//------------------------------------------------------------------------------

//------------------------------------------------------------------------------
namespace chai3d {
//------------------------------------------------------------------------------

//==============================================================================
/*!
    \file       CShapeCylinder.h

    \brief
    Implementation of a 3D cylinder object.
*/
//==============================================================================

//==============================================================================
/*!
    \class      cShapeCylinder
    \ingroup    world

    \brief
    This class implements a 3D shape cylinder.

    \details
    This class implements a 3D shape cylinder.
*/
//==============================================================================
class cShapeCylinder: public cGenericObject
{
    //--------------------------------------------------------------------------
    // CONSTRUCTOR & DESTRUCTOR:
    //--------------------------------------------------------------------------

public:

    //! Constructor of cShapeCylinder.
    cShapeCylinder(const double a_baseRadius, 
                   const double a_topRadius, 
                   const double a_height, 
                   cMaterialPtr a_material = cMaterialPtr());
    
    //! Destructor of cShapeCylinder.
    virtual ~cShapeCylinder();


    //--------------------------------------------------------------------------
    // PUBLIC METHODS:
    //--------------------------------------------------------------------------

public:

    //! This method returns the name of the object class.
    virtual std::string getClassName() { return ("ShapeCylinder"); }

    //! This method creates a copy of itself.
    cShapeCylinder* copy(const bool a_duplicateMaterialData = false,
                         const bool a_duplicateTextureData = false, 
                         const bool a_duplicateMeshData = false,
                         const bool a_buildCollisionDetector = false);

    //! This method sets the base radius of the cylinder.
    void setBaseRadius(double a_baseRadius) { m_baseRadius = fabs(a_baseRadius); updateBoundaryBox(); markForUpdate(false);}

    //! This method sets the top radius of the cylinder.
    void setTopRadius(double a_topRadius) { m_topRadius = fabs(a_topRadius); updateBoundaryBox(); markForUpdate(false);}

    //! This method sets the height of the cylinder.
    void setHeight(double a_height) { m_height = fabs(a_height); updateBoundaryBox(); markForUpdate(false);}

    //! This method returns the base radius of the cylinder.
    inline double getBaseRadius() const { return (m_baseRadius); }

    //! This method returns the top radius of the cylinder.
    inline double getTopRadius() const { return (m_topRadius); }

    //! This method returns the height of the cylinder.
    inline double getHeight() const { return (m_height); }


    //--------------------------------------------------------------------------
    // PROTECTED VIRTUAL METHODS:
    //--------------------------------------------------------------------------

protected:

    //! This method renders this object graphically using OpenGL.
    virtual void render(cRenderOptions& a_options);

    //! This method updates the boundary box of this object.
    virtual void updateBoundaryBox();

    //! This method scales the size of this object with given scale factor.
    virtual void scaleObject(const double& a_scaleFactor);

    //! This method updates the geometric relationship between the tool and the current object.
    virtual void computeLocalInteraction(const cVector3d& a_toolPos,
                                         const cVector3d& a_toolVel,
                                         const unsigned int a_IDN);

    //! This method computes collisions between a segment and this object.
    virtual bool computeOtherCollisionDetection(cVector3d& a_segmentPointA,
                                                cVector3d& a_segmentPointB,
                                                cCollisionRecorder& a_recorder,
                                                cCollisionSettings& a_settings);


    //--------------------------------------------------------------------------
    // PROTECTED METHODS:
    //--------------------------------------------------------------------------

protected:

    //! This method copies all properties of this object to another.
    void copyShapeCylinderProperties(cShapeCylinder* a_obj,
        const bool a_duplicateMaterialData,
        const bool a_duplicateTextureData, 
        const bool a_duplicateMeshData,
        const bool a_buildCollisionDetector);


    //--------------------------------------------------------------------------
    // PROTECTED MEMBERS: (cShapeCylinder)
    //--------------------------------------------------------------------------

protected:

    //! base radius of cylinder.
    double m_baseRadius;

    //! top radius of cylinder.
    double m_topRadius;

    //! height of cylinder.
    double m_height;

    //! rendering object.
    GLUquadricObj *m_quadric;
};

//------------------------------------------------------------------------------
} // namespace chai3d
//------------------------------------------------------------------------------

//------------------------------------------------------------------------------
#endif
//------------------------------------------------------------------------------
