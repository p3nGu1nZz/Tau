# HOLOGRAPHIC PROJECTION TO AN ARBITRARY PLANE FROM SPHERICAL NEAR-FIELD MEASUREMENTS 

Allen C. Newell, Bert Schlüper Nearfield Systems Inc., 1330 East 223 rd St. Suite 524, Carson, CA 90745 Robert J. Davis The Mitre Corp. 202 Burlington Road, Bedford, MA 01730-1420 

Abstract Holographic back-projections of planar near-field measurements to a plane have been available for some time. It is also straightforward to produce a hologram from cylindrical measurements to another cylindrical surface and from spherical measurements to another spherical surface 1-7 . In many cases the AUT is approximately a planar structure and it is desirable to calculate the hologram on a planar surface from cylindrical or spherical near-field or far-field measurements. This paper will describe a recently developed spherical hologram calculation where the far-field pattern can be projected on any plane by specifying the normal to the plane. The resulting hologram shows details of the radiating antenna as well as the energy scattered from the supporting structure. Since the hologram is derived from pattern data over a complete hemisphere, it generally shows more detail than holograms from planar measurements made at the same separation distance. 

Keywords: Antenna measurements; Spherical near-field; Measurement diagnostics. 

1.0 Introduction 

For reference, the planar near-field hologram will first be defined. Let us represent the measured planar near-field data on the plane z = d as B(x,y,d) where x,y defines the position of the probe on the measurement plane. From the near-field data we first compute the plane wave spectrum of the measured data using the Fast Fourier Transform (FFT). If we denote the spectrum of the measured data as ( , )x yD k k , the transmitting vector plane wave spectrum of the AUT as ( , )x yt k k

# r , and the receiving vector plane wave spectrum of the probe as ( , )x ys k k

r , the coupling equation is  

> ()2

( , ) ( , ) ( , )( , , )4        

> xyxyxyxyi d i xk yk

D k k t k k s k ke B x y d e dx dy 

> γ

## π− − += •= ∫

r r

(1) In the above equation, ( , )x yk k represents the x-y components of the propagation vector on the x-y plane and the FFT output is automatically given as a function of these variables. The time convention i te 

> ω−

is used throughout. If the Hologram is computed before probe correction, we use the calculated spectrum and the inverse Fourier transform to obtain ( )( , , ) ( , ) x yh i xk yk i d h x y x yB x y d D k k e e dk dk  

> γ+

= ∫ . (2) The results of this calculation can be interpreted as equivalent to what the probe would measure if the planar measurement had been performed at z = d h . This calculation is straightforward and fairly simple since we automatically have the plane wave spectrum as a function of the x-y components of the propagation vector. If we perform the probe correction on the measured spectrum, but do not multiply by the Cos( θ) to obtain the electric field, the hologram of the A-component of the probe-corrected spectrum is, ( )( , , ) ( , ) x yh i xk yk i d I h A x y x yB x y d t k k e e dk dk  

> γ+

= ∫ ,(3) where ( , , )I hB x y d is the new hologram. This result can be interpreted as equivalent to what an ideal, A polarized probe would measure on the plane z = d h . In general we may use any component of the probe-corrected spectrum in (3), and the results will represent the vector component that an ideal, perfectly polarized probe would measure on a plane. For instance, the AUT may be nominally linearly polarized, but if we use circular components in the probe correction, and use the right hand circular component in Equation (3), the results will represent what a point source probe that is right circularly polarized would measure on the new plane. If the electric field has been calculated by multiplying the spectrum by Cos( θ), or if the electric field is obtained from another measurement, we must first convert to the plane wave spectrum before computing the hologram. If the electric field is given as a function of the plane wave vector components, this conversion is given by ( , )( , ) ( ) ( , )( , ) ( )         

> AxyAxyExyExy

k kt k k Cos k kt k k Cos 

## θθΕ=Ε=

(4) where the subscripts A and E denote the azimuth and elevation vector components for an azimuth over elevation coordinate system. In general, any orthogonal vector components can be used. After the conversion to plane wave components using equation (4) we then use Equation (3) to obtain the hologram. Since Cos(90) = 0, we can only use the electric field values over a region slightly less than the forward hemisphere where θ < 90 degrees. A test for this condition must be included in the computer program that implements Equation (4). If the electric field is not already given as at equally spaced intervals of the plane wave vector components, we must first interpolate from the given angle coordinates to ( , )x yk k coordinates. This is necessary for instance if the electric field data comes from far-field measurements, spherical near-field or cylindrical near-field measurements. In those cases, the results are generally the electric field components as a function of spherical angles such as 

# ( ),A

## θ φ Ε , ( ),A A E Ε or ( ),A

## α ε Ε . The equations that are used for the interpolation from angular coordinates to plane wave vector coordinates are sin cos cos sin sin ,sin sin sin cos sin ,cos cos cos cos cos .

> xyz

k E Akk Ekk E Ak

## θ φαθ φα εγ θα ε= = == = == = = =

. (5) and the interpolation is illustrated schematically in Figure 1 for θ,φ coordinates . The lines of the rectangular grid represent the x-y plane where the plane wave vector components ( , )x yk k are defined. The hologram is also calculated on this plane. 

2.0 General Hologram on arbitrary plane .The procedure for obtaining a general hologram on an arbitrary plane will follow the same general approach as outlined above. The only difference will be in the specific equations that are used to convert from electric field to plane wave spectrum and to interpolate from angle coordinates to plane wave components. In this case, the plane used to define the plane wave components will not in general be the x-y plane. It will instead be the plane on 

Figure 1 Schematic of interpolation from spherical theta, phi coordinates to plane-wave vectors kx , ky.   

> kykx

which the hologram is desired and will be referred to as the holographic plane. For illustration purposes, let us assume that the far electric field of the AUT has been determined over a complete sphere using either spherical near-field measurements or far-field measurements. It is assumed that both the amplitude and phase of the far-field are known as a function of spherical angles ( θ,φ). We will denote these results as 

# ( ) ( ), , ,

> θφ

## θ φ θ φ Ε Ε . Next we define a reference direction, denoted as me

v , that is specified by the angles 

# ( ),m m

## θ φ such that the plane on which the hologram is desired is normal to the unit vector me

v . This reference direction will usually be the direction of the main beam or the normal to the AUT aperture. In general, an infinite number of coordinate systems are normal to me

v , each one differing by the rotation of the plane about the unit vector. To make the coordinates specific, we define unit vectors 1e

v and 2e

v such that they are parallel to the θ- and φ field vectors ( ),m mE 

> θ

## θ φ

r and ( ),m mE 

> φ

## θ φ

r as illustrated by Figure 2. Mathematical expressions for the unit vectors are obtained from Equation (5) and the equations that define the vector components for the different coordinate systems. For the 

θ,φ coordinate system, the x, y and z components of the unit vectors are 

# ( )1cos( ) cos( ), cos( ) sin( )sin( )    

> mmmmmmm

e

## θ φθ φθ φθ

  =   − 

v (6) 

# ( )2sin( )cos( )0 

> mmm

e

## φφφ−  =    

v (7) 

# ( )

sin( ) cos( ), sin( )sin( )cos( )     

> mmmmmmmm

e

## θ φθ φθ φθ

  =    

v (8) Calculation of the dot and cross products of these unit vectors will confirm that they form an orthogonal set and therefore 1e

v and 2e

v define the holographic plane normal to 

> m

e

v on which the hologram is to be calculated. This holographic plane is illustrated schematically in Figure 3. The solid circle represents the direction ( ),m m

## θ φ and the broad lines represent the rectangular coordinate system 

Figure 2 Illustration of unit vectors e 1 and e 2 that define the holographic plane Figure 3 Schematic of the holographic plane and the rectangular coordinates for the plane. 

YZe1e2emYZnormal to this direction which defines the holographic plane. The origin of the new coordinates is coincident with the origin of the original measurement coordinates. The x-, y- and z-components of a propagation vector for an arbitrary plane wave as defined in the original measurement coordinate system are 

# ( )

sin( ) cos( ) , sin( ) sin( ) cos( ) 

kk

## θ φθ φ θ φθ

  =    

rr . (9) The components of the same propagation vector in the holographic plane coordinate system are 

# ( ) ( )( ) ( ) ( )

> 11

cos( ) cos( )sin( ) cos( ) sin( )sin( ) cos( ) sin( )cos( ) sin( )cos cos sin( ) cos( ) cos sin sin( )sin( ) sin cos( )      

> mmmmmmmmmm

k k ek k

## θ φθ φθ φθ φθθθ φ θ φθ φ θ φ θ θ

     = • = •       −   

=+ −

r vr r

(10) 

# ( ) ( )

> 22

sin( )sin( ) cos( ) sin( ) sin( ) cos( )cos( ) 0sin sin( ) cos( ) cos sin( ) sin( )  

> mmmm

k k ek k

φθ φθ φφθφ θ φφ θ φ−     = • = •         

= − +

r vr r

(11) 

# ( ) ( )( ) ( ) ( )

sin( ) cos( )sin( ) cos( ) sin( ) sin( ) sin( ) sin( )cos( ) cos( )sin cos sin( ) cos( ) sin sin sin( )sin( ) cos cos( )       

> mmmmmmmmmmmm

k k ek k

## θ φθ φθ φθ φθθθ φ θ φθ φ θ φ θ θ

     = • = •         

=+ +

r vr r

(12) The notation showing that the vectors are functions of ( θ,φ)or ( θm, φm) has been deleted for brevity. To compute the hologram on the newly defined plane we must first convert from electric field components to plane wave components defined in the holographic coordinate system using an equation similar to (4). This requires knowing the angle between the reference direction and the direction of propagation for each plane wave. We note that the angle between the reference direction and the propagation vector, and denoted by Θ , that corresponds to 

θ for the original measurement system is 

# ( )( ) ( )( ) ( )( )

cos sin( ) cos( )sin( ) cos( ) sin( ) sin( ) sin( )sin( )cos( ) cos( )sin cos sin( ) cos( ) sin sin sin( )sin( ) cos cos( )     

> mmmmmmmmmmm

k ek

## θ φθ φθ φθ φθθθ φ θ φθ φ θ φθ θΘ = •

     = •         

=+−

r vr

(13) And therefore the conversion from electric field to plane wave spectrum is given by 

# ( ) ( )( )

,, cos 

t

## θ φ θ φ Ε= Θ

rr (14) As in the case of the hologram on the x-y plane, this equation is only valid for the "hemisphere" region where the denominator is non-zero. The boundaries of this region are defined by the condition that [ ]

cos( )tan( ) sin( ) cos( ) cos( ) sin( )sin( )   

> mbnd mmm

θθ θ φ φ φ φ−= +

(15) To complete the calculation of the hologram, we then use Equations (10) and (11) to interpolate from (θ,φ)coordinates to (k 1 ,k 2 ) coordinates which gives ( )1 2,t k k

r .Equation (3) is then used to calculate the hologram on the holographic plane for one or more of the vector components of plane wave spectrum. The distance d h is the distance from the origin of the spherical coordinates to the surface of the antenna and may be either positive or negative depending on the location of the antenna. 

3.0 Measurement Results 

Three measurements were performed to illustrate the features of the hologram from spherical data. The first antenna shown in Figure 4 is an X-Band slotted array with major dimensions of approximately 10 wavelengths. Conductive tape was placed over two elements to simulate faults in the antenna. Both spherical and planar near-field measurements were performed on this antenna with the probe approximately 22 wavelengths from the antenna. At this distance, the far-field from the planar data was only valid to 40 degrees off-axis. As a result, the hologram from the planar data showed little detail of the array elements and the null at the fault was not very sharp as show in Figure 5. The spherical hologram showed much more detail of both the element locations and the deep, sharp null at the simulated fault. The greater detail in the spherical hologram is possible since accurate pattern data is available over the complete hemisphere normal to the antenna. The wide-angle portions of the pattern produce the fine detail in the hologram. The setup for the second measurement is shown in Figure 6. Even though the antenna is small, the airplane model that it is mounted on has some effect on the pattern as currents are induced on the airplane, and so measurements must be made at 0.5-degree intervals. The measurements were performed at 15.75 GHz over the complete sphere and the hologram was computed for the plane normal to the Z-axis. One useful feature of the hologram is that it can be calculated for planes with different dimensions. A small plane centered on the radiating elements will show the amplitude and phase distribution for the actual antenna. If the size of the plane is increased to include the major 

Figure 4 Slotted array antenna with conductive tape over an element to simulate a fault. Figure 5 Comparison of holograms from planar and spherical near-field measurements. Figure 6 Aircraft model with small antenna on spherical near-field range.       

> -6 -4 -2 0246Y-Position in Wavelengths -30 -20 -10 0Relative Amplitude in dB Spherical Data Planar Data

portion of the aircraft and the dynamic range of the hologram is increased to show very low signal levels, the scattering from the aircraft can be clearly seen. The detail and large dynamic range results from a combination of the fine data point spacing, the low noise level in the measured data and the spherical data over the complete hemisphere that encloses the hologram plane. Both of the holograms in Figures 5 and 7 have been calculated for a plane normal to the Z-axis where the interpolation from angles to k1 and k2 are fairly simple. As a test of the general hologram, a fan beam antenna was mounted with the peak of the main beam near theta = 90, phi = 0. This orientation of the antenna is referred to as an equatorial mount. The hologram for this antenna is shown in Figure 8 and demonstrates again the sharp resolution of the spherical hologram. The relative amplitude of the individual elements in the antenna is also apparent and can be used to make adjustments in element amplitude and phase. 

4.0 Conclusions 

Spherical near-field measurements can be used to obtain high-resolution holographic projections to any arbitrary plane. Measurements have been presented to illustrate the technique for planes normal to the Z-axis and normal to the X-axis. 

5.0 References 

[1] Langsford, P.A.; Hayes, M.J.C.; Henderson, R., “Holographic diagnostics of a phased array antenna from near field measurements”. AMTA 1989, p. 10-32. [2] Repjar, A.; Guerrieri, J.; Kremer, D.; Canales, N.; Wilkes, R.J., “Determining faults on a flat phased array antenna using planar near-field techniques”. AMTA 1991, p. 8-11. [3] Guler, M.G.; Joy, E.B.; Black, D.N.; Wilson, R.E., “Far-field spherical microwave holography”. AMTA 1992, p. 8-3. [4] Rochblatt, D.J.; Seidel, B.L., “Microwave antenna holography”, Microwave Theory and Techniques, IEEE Transactions on , Volume: 40 Issue: 6 , June 1992, Page(s): 1294 –1300. [5] Isernia, T.; Leone, G.; Pierri, R.; Soldovieri, F., “Microwave diagnostics by holography and phase retrieval”. AMTA 1994, p. 244. [6] Farhat, K.S.; Williams, N., “Microwave holography applications in antenna development”, Novel Antenna Measurement Techniques, IEE Colloquium on , 1994 Page(s): 3/1 -3/4. [7] Guler, M.G.; Joy, E.B., “High resolution spherical microwave holography”, Antennas and Propagation, IEEE Transactions on , Volume: 43 Issue: 5 , May 1995, Page(s): 464 –472. 

Figure 7 Hologram amplitude from spherical near-field data on aircraft model. Figure 8 Hologram for plane normal to theta = 90 degrees for fan beam antenna.