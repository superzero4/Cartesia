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
    \version   3.3.0
*/
//==============================================================================

//------------------------------------------------------------------------------
#ifndef CShapeLineH
#define CShapeLineH
//------------------------------------------------------------------------------
#include "world/CGenericObject.h"
#include "materials/CMaterial.h"
//------------------------------------------------------------------------------

//------------------------------------------------------------------------------
namespace chai3d {
//------------------------------------------------------------------------------

//==============================================================================
/*!
    \file       CShapeLine.h

    \brief 
    Implementation of a 3D line object.
*/
//==============================================================================

//==============================================================================
/*!
    \class      cShapeLine
    \ingroup    world

    \brief
    This class implements a 3D line.

    \details
    This class implements a 3D line.
*/
//==============================================================================
class cShapeLine : public cGenericObject
{  
    //--------------------------------------------------------------------------
    // CONSTRUCTOR & DESTRUCTOR:
    //--------------------------------------------------------------------------

public:

    //! Constructor of cShapeLine.
    cShapeLine();

    //! Constructor of cShapeLine.
    cShapeLine(const cVector3d& a_pointA, const cVector3d& a_pointB);

    //! Constructor of cShapeLine.
    cShapeLine(cGenericObject* a_objectA, cGenericObject* a_objectB);

    //! Constructor of cShapeLine.
    cShapeLine(cGenericObject* a_objectA, const cVector3d& a_pointA,
        cGenericObject* a_objectB, const cVector3d& a_pointB);

    //! Destructor of cShapeLine.
    virtual ~cShapeLine() {};


    //--------------------------------------------------------------------------
    // PUBLIC METHODS:
    //--------------------------------------------------------------------------

public:

    //! This method returns the name of the object class.
    virtual std::string getClassName() { return ("ShapeLine"); }

    //! This method creates a copy of itself.
    cShapeLine* copy(const bool a_duplicateMaterialData = false,
                     const bool a_duplicateTextureData = false, 
                     const bool a_duplicateMeshData = false,
                     const bool a_buildCollisionDetector = false);

    //! This method sets the line width.
    inline void setLineWidth(const double a_lineWidth) { m_lineWidth = fabs(a_lineWidth); }

    //! This method returns the line width.
    inline double getLineWidth() const { return (m_lineWidth); }

    //! This method sets the line properties.
    void setLineStipple(const GLint a_stippleFactor, const GLushort a_stipplePattern);

    //! This method returns the line stipple factor value.
    GLint getLineStippleFactor() { return (m_stippleFactor); }

    //! This method returns the stipple pattern value.
    GLushort  getLineStipplePattern() { return (m_stipplePattern); }

    //! This method updates position of line extremity points.
    void updateLinePoints();


    //--------------------------------------------------------------------------
    // PUBLIC MEMBERS: (cShapeLine)
    //--------------------------------------------------------------------------

public:

    //! Point A of line.
    cVector3d m_pointA;

    //! Point A of line.
    cVector3d m_pointB;

    //! Color of point A of line.
    cColorf m_colorPointA;

    //! Color of point B of line.
    cColorf m_colorPointB;

    //! Object A to wich line extremity is attached.
    cGenericObject* m_objectA;

    //! Object B to wich line extremity is attached.
    cGenericObject* m_objectB;


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


    //-----------------------------------------------------------------------
    // PROTECTED METHODS:
    //-----------------------------------------------------------------------

protected:

    //! This method copies all properties of this object to another.
    void copyShapeLineProperties(cShapeLine* a_obj,
        const bool a_duplicateMaterialData,
        const bool a_duplicateTextureData, 
        const bool a_duplicateMeshData,
        const bool a_buildCollisionDetector);

    // This method initializes this shape line object.
    void reset();


    //--------------------------------------------------------------------------
    // PROTECTED MEMBERS:
    //--------------------------------------------------------------------------

protected:

    //! Line width
    double m_lineWidth;

    //! Specifies a multiplier for each bit in the line stipple pattern.
    GLint m_stippleFactor;

    //! Specifies a 16-bit integer whose bit pattern determine which fragments of a line will be drawn when the line is rasterized.
    GLushort m_stipplePattern;

    // Point A in local coordinates
    cVector3d m_linePointA;

    // Point A in local coordinates
    cVector3d m_linePointB;

};

//------------------------------------------------------------------------------
} // namespace chai3d
//------------------------------------------------------------------------------

//------------------------------------------------------------------------------
#endif
//------------------------------------------------------------------------------
